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
	[Export] private PackedScene _displayUI_Prefab;
	[Export] private GridContainer[] _buildingPickContainers;
	[Export] private InfoScreen _infoScreenScreen;

	private Action<int?> _dataChanged;
	private readonly Dictionary<string, int> _hotkeys = new();

	public override void _Ready() => PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

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
		
		switch (Visible)
		{
			case false when eventKey.Keycode == Key.B:
				Visible = true;
				return;
			case true when eventKey.Keycode == Key.Escape:
				Visible = false;
				_dataChanged?.Invoke(null);
				break;
		}

		if (!_hotkeys.TryGetValue(eventKey.Keycode.ToString(), out int hotkey)) 
			return;

		foreach (GridContainer buildingPickContainer in _buildingPickContainers)
			if (buildingPickContainer.GetChildren().FirstOrDefault(child => child.Name == hotkey.ToString()) is DisplayUI buttonUI)
			{
				_dataChanged?.Invoke(buttonUI.Id);
				break;
			}
	}

	private void InitializeBuildingPicks(Action<int?> dataChanged)
	{
		_dataChanged = dataChanged;
		List<DisplayUI> buildingsUI = new();
		
		Building[] buildings = ItemDatabase.GetAllOfType<Building>();
		foreach (Building building in buildings)
		{
			if (_displayUI_Prefab.Instantiate() is not DisplayUI uiItem)
				continue;
			
			uiItem.InitializeValues(building, building.Hotkey);
			uiItem.Name = building.Id.ToString();

			uiItem.MouseEntered += () => _infoScreenScreen?.ShowDisplayPanel(building.Sprite, building.DisplayName, building.Description);
			uiItem.MouseExited += () => _infoScreenScreen?.HideInfoPanel();
			uiItem.Pressed += () => _dataChanged?.Invoke(uiItem.Id);

			_hotkeys.TryAdd(building.Hotkey, building.Id);
			_buildingPickContainers[(byte)building.TabType - 1].AddChild(uiItem);
			buildingsUI.Add(uiItem);
		}
	}
}