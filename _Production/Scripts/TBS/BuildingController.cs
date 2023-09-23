using System;
using FT.Data;
using FT.Data.Items;
using FT.Managers;
using FT.UI;
using Godot;

namespace FT.TBS;

public partial class BuildingController : Node
{
    private Action<int> _onBuildingIdChanged;
    
    private int? _buildingId;

    public override void _Ready()
    {
        _onBuildingIdChanged = value => _buildingId = value;
        ((PlayerUI)GetTree().GetFirstNodeInGroup(nameof(PlayerUI)))?.BuildingScreen.Initialize(_onBuildingIdChanged);
        
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(State);
    }

    private void State(StateParameters State)
    {
        State.IsMouseClicked.AddObserver(value => { if (value) TryBuild(State.RowCol.Value); });
    }

    private void TryBuild((byte? row, byte? col) value)
    {
        if (!_buildingId.HasValue || !value.row.HasValue || !value.col.HasValue)
            return;
        
        Building _building = ItemDatabase.Get<Building>(_buildingId.Value);
        UnpackedCellData data = CellManager.GetCellData((value.row.Value, value.col.Value));
        if (_building.ResourceType != data.resourceType || data.isOccupied)
            return;
        
        Node3D building3D = _building.Prefab.Instantiate() as Node3D;
        building3D!.Position = new Vector3(value.row.Value * 20 + 10, 0, value.col.Value * 20 + 10);
        AddChild(building3D);

        CellManager.SetIsOccupied(value.row.Value, value.col.Value);
        _buildingId = null;
    }
}