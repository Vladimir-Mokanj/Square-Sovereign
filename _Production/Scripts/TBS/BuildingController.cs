using System;
using FT.Data;
using FT.Data.Items;
using FT.Managers;
using FT.UI;
using Godot;

namespace FT.TBS;

public partial class BuildingController : Node
{
    private Action<int?> _onBuildingIdChanged;
    
    private int? _buildingId;
    private Node3D _ghostBuilding;

    public override void _Ready()
    {
        _onBuildingIdChanged = OnBuildingIdChanged;
        ((PlayerUI)GetTree().GetFirstNodeInGroup(nameof(PlayerUI)))?.BuildingScreen.Initialize(_onBuildingIdChanged);
        
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(OnStateAssigned);
    }

    private void OnBuildingIdChanged(int? value)
    {
        _buildingId = value;
        if (_ghostBuilding != null)
            RemoveChild(_ghostBuilding);

        if (!value.HasValue)
            return;
        
        _ghostBuilding = ItemDatabase.Get<Building>(value.Value).Prefab.Instantiate() as Node3D;
        _ghostBuilding.Visible = false;
        AddChild(_ghostBuilding);
    }

    private void OnStateAssigned(StateParameters State)
    {
        State.RowCol.AddObserver(PlaceGhostBuilding);
        State.IsMouseLeftDown.AddObserver(value => { if (value) TryBuild(State.RowCol.Value); });
        State.IsMouseRightDown.AddObserver(value =>
        {
            if (value && _buildingId.HasValue) 
                _buildingId = null;

            if (_ghostBuilding != null)
                RemoveChild(_ghostBuilding);

            _ghostBuilding = null;
        });
    }

    private void PlaceGhostBuilding((byte? row, byte? col) value)
    {
        if (_ghostBuilding == null)
            return;

        if (!value.row.HasValue || !value.col.HasValue)
            return;

        _ghostBuilding.Visible = !CellManager.GetCellData((value.row.Value, value.col.Value)).isOccupied;
        _ghostBuilding.Position = new Vector3(value.row.Value * 20 + 10, 0, value.col.Value * 20 + 10);
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
        RemoveChild(_ghostBuilding);

        CellManager.SetIsOccupied(value.row.Value, value.col.Value);
        _buildingId = null;
        _ghostBuilding = null;
    }
}