using System;
using System.Collections.Generic;
using Godot;
using FT.Data;
using FT.Data.Items;
using FT.Managers;

namespace FT.UI;

public partial class BuildingScreen : Control
{
	[Export] private PackedScene _buildingSelectionUI_Prefab;
	[Export] private PackedScene _buildingPickUI_Prefab;
	[Export] private Control _buildingSelectionControlNode;
	[Export] private Control _buildingPickControlNode;
	[Export] private Panel _BuildingInfoPanel;

	private Building _currentBuilding;

	public bool BuildStructure(byte row, byte col, byte cellSize, (TerrainType terrainType, ResourceType resourceType, bool isOccupied) data)
	{
		if (_currentBuilding == null) 
			return false;
		
		if (_currentBuilding.ResourceType != data.resourceType || data.isOccupied)
			return false;
		
		Node3D building = _currentBuilding.Prefab.Instantiate() as Node3D;
		building.Position = new Vector3(row * cellSize + cellSize/2.0f, 0, col * cellSize + cellSize/2.0f); 
		
		AddChild(building);
		return true;
	}
	
	public override void _Ready()
	{
		List<BuildingUI> createdBuildingUI = InitializeBuildingPicks();
		InitializeBuildingSelection(createdBuildingUI);
	}

	private List<BuildingUI> InitializeBuildingPicks()
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
				_currentBuilding = ItemDatabase.Get<Building>(uiItem.ID);
				(_BuildingInfoPanel as BuildingInfo)?.SetDisplayValues(_currentBuilding.Sprite, _currentBuilding.DisplayName, _currentBuilding.Description);
			};
			
			_buildingPickControlNode.AddChild(uiItem);
			buildingsUI.Add(uiItem);
		}

		return buildingsUI;
	}

	private void InitializeBuildingSelection(List<BuildingUI> createdBuildingUI)
	{
		BuildingType _buildingType = BuildingType.NONE;
		foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
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
				(_BuildingInfoPanel as BuildingInfo)?.SetDisplayValues(null, "", "");
				if (_buildingType == BuildingType.NONE) _currentBuilding = null;
				createdBuildingUI.ForEach(buildingUi => buildingUi.Visible = _buildingType == ItemDatabase.Get<Building>(buildingUi.ID).TabType);
			};
		}
	}
}