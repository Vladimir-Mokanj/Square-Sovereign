using System;
using FT.Data;
using FT.Data.Items;
using FT.Managers;
using FT.UI;
using Godot;

namespace FT.TBS;

public partial class BuildingController : Node
{
    [Export] private BuildingScreen _buildingScreen;
    private Action<int> _onBuildingIdChanged;
    private Building _building;

    public override void _Ready()
    {
        _onBuildingIdChanged = value => _building = ItemDatabase.Get<Building>(value);
        _buildingScreen.Initialize(_onBuildingIdChanged);
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(State);
    }

    private void State(StateParameters State) => 
        State.IsMouseClicked.AddObserver(Build);

    private void Build(bool value)
    {
        if (!value || _building == null)
            return;
        
        GD.PrintErr("Yes");
        _building = null;
    }
    
    private bool BuildStructure(byte row, byte col, byte cellSize, (TerrainType terrainType, ResourceType resourceType, bool isOccupied) data)
    {
        if (_building.ResourceType != data.resourceType || data.isOccupied)
            return false;
	
        Node3D building3D = _building.Prefab.Instantiate() as Node3D;
        building3D.Position = new Vector3(row * cellSize + cellSize/2.0f, 0, col * cellSize + cellSize/2.0f); 
	
        AddChild(building3D);
        return true;
    }
}