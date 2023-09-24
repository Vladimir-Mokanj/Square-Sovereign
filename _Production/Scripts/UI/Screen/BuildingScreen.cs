using System;
using System.Collections.Generic;
using Godot;
using FT.Data;
using FT.Data.Items;
using FT.TBS;

namespace FT.UI;

public partial class BuildingScreen : Control
{
	[Export] private PackedScene _buildingSelectionUI_Prefab;
	[Export] private PackedScene _displayUI_Prefab;
	[Export] private Control _buildingSelectionControlNode;
	[Export] private Control _buildingPickControlNode;
	[Export] private InfoScreen _infoScreenScreen;

	public override void _Ready() => PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateInitialized);

	private void OnStateInitialized(StateParameters State)
	{
		State.BuildingSelectedID.AddObserver(i => Visible = !i.HasValue);
		State.IsMouseRightDown.AddObserver(b => { if (b) Visible = true; });
	}

	public void Initialize(Action<int?> dataChanged)
	{
		List<BuildingUI> createdBuildingUI = InitializeBuildingPicks(dataChanged);
		InitializeBuildingSelection(createdBuildingUI, dataChanged);
	}

	private List<BuildingUI> InitializeBuildingPicks(Action<int?> dataChanged)
	{
		List<BuildingUI> buildingsUI = new();
		
		Building[] buildings = ItemDatabase.GetAllOfType<Building>();
		foreach (Building building in buildings)
		{
			if (_displayUI_Prefab.Instantiate() is not BuildingUI uiItem)
				continue;
			
			uiItem.InitializeValues(building);
			uiItem.Visible = false;

			uiItem.MouseEntered += () =>
			{
				Building _currentBuilding = ItemDatabase.Get<Building>(uiItem.ID);
				_infoScreenScreen?.ShowDisplayPanel(_currentBuilding.Sprite, _currentBuilding.DisplayName, _currentBuilding.Description);
			};
			
			uiItem.MouseExited += () => _infoScreenScreen?.HideInfoPanel();
			uiItem.Pressed += () => dataChanged?.Invoke(uiItem.ID);
			
			_buildingPickControlNode.AddChild(uiItem);
			buildingsUI.Add(uiItem);
		}

		return buildingsUI;
	}

	private void InitializeBuildingSelection(List<BuildingUI> createdBuildingUI, Action<int?> dataChanged)
	{
		BuildingType _buildingType = BuildingType.NONE;
		foreach (BuildingType type in (BuildingType[]) Enum.GetValues(typeof(BuildingType)))
		{
			if (type == BuildingType.NONE)
				continue;

			if (_buildingSelectionUI_Prefab.Instantiate() is not Button selection)
				continue;
			
			selection.Text = type.ToString();
			_buildingSelectionControlNode.AddChild(selection);
			selection!.Pressed += () =>
			{
				_buildingType = _buildingType.ToString() == selection.Text ? BuildingType.NONE : type;
				dataChanged?.Invoke(null);
				createdBuildingUI.ForEach(buildingUi => buildingUi.Visible = _buildingType == ItemDatabase.Get<Building>(buildingUi.ID).TabType);
			};
		}
	}
}