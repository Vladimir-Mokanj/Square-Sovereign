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

	public void Initialize(Action<int> dataChanged)
	{
		List<BuildingUI> createdBuildingUI = InitializeBuildingPicks(dataChanged);
		InitializeBuildingSelection(createdBuildingUI);
	}

	private List<BuildingUI> InitializeBuildingPicks(Action<int> dataChanged)
	{
		List<BuildingUI> buildingsUI = new();
		
		Building[] buildings = ItemDatabase.GetAllOfType<Building>();
		foreach (Building building in buildings)
		{
			if (_buildingPickUI_Prefab.Instantiate() is not BuildingUI uiItem)
				continue;
			
			uiItem.InitializeValues(building);
			uiItem.Visible = false;
			uiItem.Pressed += () =>
			{
				Building _currentBuilding = ItemDatabase.Get<Building>(uiItem.ID);
				dataChanged?.Invoke(uiItem.ID);
				_BuildingInfoPanel?.SetDisplayValues(_currentBuilding.Sprite, _currentBuilding.DisplayName, _currentBuilding.Description);
			};
			
			_buildingPickControlNode.AddChild(uiItem);
			buildingsUI.Add(uiItem);
		}

		return buildingsUI;
	}

	private void InitializeBuildingSelection(List<BuildingUI> createdBuildingUI)
	{
		BuildingType _buildingType = BuildingType.NONE;
		
		BuildingType[] buildingTypes = (BuildingType[]) Enum.GetValues(typeof(BuildingType));
		foreach (BuildingType type in buildingTypes)
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
				createdBuildingUI.ForEach(buildingUi => buildingUi.Visible = _buildingType == ItemDatabase.Get<Building>(buildingUi.ID).TabType);
			};
		}
	}
}