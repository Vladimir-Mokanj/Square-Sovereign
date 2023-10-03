using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using FT.Data;
using FT.Data.Items.Civilization;
using FT.TBS;

namespace FT.UI;

public partial class BuildingScreen : Control
{
	[Export] private Button _buildButton;
	[Export] private HBoxContainer _buildingsContainer;
	
	[Export] private PackedScene _displayUI_Prefab;
	[Export] private GridContainer[] _buildingPickContainers;
	[Export] private InfoScreen _infoScreenScreen;

	private Action<int?> _dataChanged;
	private readonly Dictionary<string, int> _hotkeys = new();
	private Tween _tween;

	public override void _Ready() => 
		PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

	private void OnStateInitialized(StateParameters State)
	{
		State.BuildingSelectedID.AddObserver(i => { Visible = !i.HasValue; });
		State.IsMouseRightDown.AddObserver(b => { if (b) Visible = true; });
	}

	public void Initialize(Action<int?> dataChanged)
	{
		foreach (GridContainer buildingPickContainer in _buildingPickContainers)
			foreach (Node node in buildingPickContainer.GetChildren())
				node.QueueFree();

		InitializeBuildingPicks(dataChanged);
	}

	
	public override void _Input(InputEvent @event)
	{
		if (@event is not InputEventKey { Pressed: true } eventKey) 
			return;

		if (_tween != null || !Visible)
			return;
		
		_dataChanged?.Invoke(null);
		switch (_buildingsContainer.Visible)
		{
			case false when eventKey.Keycode == Key.B:
				ToggleVisibility();
				return;
			case true when eventKey.Keycode == Key.Escape:
				ToggleVisibility();
				break;
		}

		if (!_hotkeys.TryGetValue(eventKey.Keycode.ToString(), out int hotkey)) 
			return;

		foreach (GridContainer buildingPickContainer in _buildingPickContainers)
			if (buildingPickContainer.GetChildren().FirstOrDefault(child => child.Name == hotkey.ToString()) is DisplayUI buttonUI)
			{
				_dataChanged?.Invoke(buttonUI.Id);
				ToggleVisibility();
				break;
			}
	}

	private void ToggleVisibility()
	{
		Vector2 buildingsContainerPosition = _buildingsContainer.Visible ? _buildingsContainer.Position + new Vector2(0, 112) : _buildingsContainer.Position - new Vector2(0, 112);
		Vector2 buildButtonPosition = _buildButton.Visible ? _buildButton.Position + new Vector2(0, 80) : _buildButton.Position - new Vector2(0, 80);

		Control visibleNode = _buildButton.Visible ? _buildButton : _buildingsContainer;
		_buildButton.Visible = true;
		_buildingsContainer.Visible = true;
		
		_tween?.Kill();
		_tween = CreateTween().SetParallel();
		_tween.TweenProperty(_buildingsContainer, "position", buildingsContainerPosition, 0.04f);
		_tween.TweenProperty(_buildButton, "position", buildButtonPosition, 0.04f);
		_tween.Finished += () => { visibleNode.Visible = false; _tween = null; };
	}

	private void InitializeBuildingPicks(Action<int?> dataChanged)
	{
		_dataChanged = dataChanged;
		Building[] buildings = ItemDatabase.GetAllOfType<Building>();
		foreach (Building building in buildings)
		{
			if (_displayUI_Prefab.Instantiate() is not DisplayUI uiItem)
				continue;
			
			uiItem.InitializeValues(building, building.Hotkey);
			uiItem.Name = building.Id.ToString();

			uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(building.Sprite, building.DisplayName, building.Description);
			uiItem.MouseExited += () => _infoScreenScreen?.HideInfoPanel();
			uiItem.Pressed += () => { ToggleVisibility(); _dataChanged?.Invoke(uiItem.Id); };

			_hotkeys.TryAdd(building.Hotkey, building.Id);
			_buildingPickContainers[(byte)building.TabType - 1].AddChild(uiItem);
			SetColumnSize(_buildingPickContainers[(byte)building.TabType - 1]);
		}
	}
	
	private void SetColumnSize(GridContainer container) => 
		container.Columns = Mathf.CeilToInt(container.GetChildCount() / 2.0f);
}