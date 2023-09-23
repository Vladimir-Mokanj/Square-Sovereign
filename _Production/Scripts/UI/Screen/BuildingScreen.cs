using System;
using System.Collections.Generic;
using Godot;
using FT.Data;
using FT.Data.Items;

namespace FT.UI;

public partial class BuildingScreen : Control
{
	[Export] private PackedScene _buildingSelectionUI_Prefab;
	[Export] private PackedScene _buildingPickUI_Prefab;
	[Export] private Control _buildingSelectionControlNode;
	[Export] private Control _buildingPickControlNode;
	[Export] private BuildingInfo _BuildingInfoPanel;

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
			if (_buildingPickUI_Prefab.Instantiate() is not BuildingUI uiItem)
				continue;
			
			uiItem.InitializeValues(building);
			uiItem.Visible = false;

			uiItem.MouseEntered += () =>
			{
				Building _currentBuilding = ItemDatabase.Get<Building>(uiItem.ID);
				_BuildingInfoPanel?.ShowDisplayPanel(_currentBuilding.Sprite, _currentBuilding.DisplayName, _currentBuilding.Description);
			};
			
			uiItem.MouseExited += () => _BuildingInfoPanel?.HideInfoPanel();
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