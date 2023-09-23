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
    private CellManager _cellManager;
    
    public override void _Ready()
    {
        _onBuildingIdChanged = value => _building = ItemDatabase.Get<Building>(value);
        _buildingScreen.Initialize(_onBuildingIdChanged);
        
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(State);
    }
    
    public void Initialize(CellManager cellManager) => 
        _cellManager = cellManager;

    private void State(StateParameters State) => 
        State.IsMouseClicked.AddObserver(value => { if (value) TryBuild(State.RowCol.Value); });

    private void TryBuild((byte? row, byte? col) value)
    {
        GD.PrintErr("Hello");
        if (_building == null || !value.row.HasValue || !value.col.HasValue)
            return;
        
        if (DidBuildStructure((value.row.Value, value.col.Value)))
            _building = null;
    }
    
    private bool DidBuildStructure((byte row, byte col) value)
    {
        UnpackedCellData data = _cellManager.GetCellData(value);
        if (_building.ResourceType != data.resourceType || data.isOccupied)
            return false;
	
        Node3D building3D = _building.Prefab.Instantiate() as Node3D;
        building3D.Position = new Vector3(value.row * 20 + 10, 0, value.col * 20 + 10); 
	
        AddChild(building3D);
        return true;
    }
}