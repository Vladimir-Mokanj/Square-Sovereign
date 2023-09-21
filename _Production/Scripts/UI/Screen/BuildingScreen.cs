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
			uiItem.Pressed += () => _currentBuilding = ItemDatabase.Get<Building>(uiItem.ID);
			uiItem.Visible = building.TabType == BuildingType.ECONOMY;
			
			_buildingPickControlNode.AddChild(uiItem);
			buildingsUI.Add(uiItem);
		}

		return buildingsUI;
	}

	private void InitializeBuildingSelection(List<BuildingUI> createdBuildingUI)
	{
		foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
		{
			if (_buildingSelectionUI_Prefab.Instantiate() is not Button selection)
				continue;
			
			selection.Text = type.ToString();
			_buildingSelectionControlNode.AddChild(selection);
			selection!.Pressed += () => createdBuildingUI.ForEach(buildingUi => buildingUi.Visible = ItemDatabase.Get<Building>(buildingUi.ID).TabType == type);
		}
	}
}