using System;
using FT.Data;
using FT.Data.Items.Civilization;
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
        if (value == _buildingId)
            return;
        
        _buildingId = value;
        if (!value.HasValue)
            return;
        
        if (_ghostBuilding != null)
            RemoveChild(_ghostBuilding);

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
        State.BuildingSelectedID.AddObserver(RemoveGhostBuilding);
    }

    private void RemoveGhostBuilding(int? value)
    {
        if (!value.HasValue || _ghostBuilding == null)
            return;
        
        RemoveChild(_ghostBuilding);
        _ghostBuilding = null;
    }

    private void PlaceGhostBuilding((byte? row, byte? col) value)
    {
        if (_ghostBuilding == null)
            return;

        if (!value.row.HasValue || !value.col.HasValue)
            return;
        
        _ghostBuilding.Visible = CanPlaceBuilding((value.row.Value, value.col.Value));
        _ghostBuilding.Position = new Vector3(value.row.Value * 20 + 10, 0, value.col.Value * 20 + 10);
    }

    private void TryBuild((byte? row, byte? col) value)
    {
        if (!_buildingId.HasValue || !value.row.HasValue || !value.col.HasValue)
            return;

        if (!CanPlaceBuilding((value.row.Value, value.col.Value)))
            return;

        CellManager.SetIsOccupied(value.row.Value, value.col.Value);
        _ghostBuilding.Name = $"{_buildingId.Value.ToString()}|";

        _buildingId = null;
        _ghostBuilding = null;
    }

    private bool CanPlaceBuilding((byte row, byte col) value)
    {
        Building _building = _buildingId.HasValue ? ItemDatabase.Get<Building>(_buildingId.Value) : null;
        if (_building == null)
            return false;
        
        UnpackedCellData data = CellManager.GetCellData((value.row, value.col));
        return _building.ResourceType == data.resourceType && !data.isOccupied;
    }
}