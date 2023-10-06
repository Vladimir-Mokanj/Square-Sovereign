using System;
using System.Collections.Generic;
using System.Linq;
using FT.Data;
using FT.Data.Items.Civilization;
using FT.Managers;
using FT.UI;
using Godot;

namespace FT.TBS;

internal struct WhileBuilding
{
    internal readonly Node3D originalBuilding;
    internal readonly Node3D tempBuilding;

    public WhileBuilding(Node3D originalBuilding, Node3D tempBuilding)
    {
        this.originalBuilding = originalBuilding;
        this.tempBuilding = tempBuilding;
    }
}

public partial class BuildingController : Node
{
    [Export] private float _transparentValue = 0.8f;
    [Export] private PackedScene _tempBuilding;
    
    private Action<int?> _onBuildingIdChanged;
    
    private int? _buildingId;
    private Node3D _ghostBuilding;
    private (byte? row, byte? col) rowCol;
    private readonly List<(ushort buildingDone, WhileBuilding building)> _buildings = new();

    public override void _Ready()
    {
        InitializeBuildScreen();
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(OnStateAssigned);
    }

    public void InitializeBuildScreen()
    {
        _onBuildingIdChanged -= OnBuildingIdChanged;
        _onBuildingIdChanged += OnBuildingIdChanged;
        ((PlayerUI)GetTree().GetFirstNodeInGroup(nameof(PlayerUI)))?.BuildingScreen.Initialize(_onBuildingIdChanged);
    }

    private void OnBuildingIdChanged(int? value)
    {
        if (value == _buildingId)
            return;
        
        _buildingId = value;
        if (_ghostBuilding != null)
        {
            RemoveChild(_ghostBuilding);
            _ghostBuilding = null;
        }
        
        if (!value.HasValue)
            return;

        _ghostBuilding = ItemDatabase.Get<Building>(value.Value).Prefab.Instantiate() as Node3D;
        AddChild(_ghostBuilding);

        if (_ghostBuilding == null || !rowCol.row.HasValue || !rowCol.col.HasValue)
            return;
        
        SetTransparency(CanPlaceBuilding((rowCol.row.Value, rowCol.col.Value)) ? 0.0f : _transparentValue);
        _ghostBuilding.GlobalPosition = new Vector3(rowCol.row.Value * 20 + 10, CellManager.GetHeight(rowCol.row.Value, rowCol.col.Value), rowCol.col.Value * 20 + 10);
    }

    private void OnStateAssigned(StateParameters State)
    {
        State.TurnNumber.AddObserver(CheckForBuilds);
        State.BuildingSelectedID.AddObserver(RemoveGhostBuilding);
        State.RowCol.AddObserver(PlaceGhostBuilding);
        State.IsMouseLeftDown.AddObserver(value => { if (value && !State.IsMouseDrag.Value) TryBuild(State.RowCol.Value, State.TurnNumber.Value); });
        State.IsMouseRightDown.AddObserver(value =>
        {
            if (value && _buildingId.HasValue) 
                _buildingId = null;

            if (_ghostBuilding != null)
                RemoveChild(_ghostBuilding);

            _ghostBuilding = null;
        });
    }

    private void CheckForBuilds(ushort turn)
    {
        if (_buildings.Count <= 0)
            return;

        foreach ((ushort buildingDone, WhileBuilding building) building in _buildings.Where(building => turn == building.buildingDone))
        {
            building.building.originalBuilding.Visible = true;
            building.building.tempBuilding.QueueFree();
        }
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
        rowCol = value;
        if (_ghostBuilding == null)
            return;

        if (!value.row.HasValue || !value.col.HasValue)
            return;

        SetTransparency(CanPlaceBuilding((value.row.Value, value.col.Value)) ? 0.0f : _transparentValue);
        _ghostBuilding.Position = new Vector3(value.row.Value * 20 + 10, CellManager.GetHeight(value.row.Value, value.col.Value), value.col.Value * 20 + 10);
    }

    private void TryBuild((byte? row, byte? col) value, ushort currentTurn)
    {
        if (!_buildingId.HasValue || !value.row.HasValue || !value.col.HasValue)
            return;

        if (!CanPlaceBuilding((value.row.Value, value.col.Value)))
            return;

        CellManager.SetIsOccupied(value.row.Value, value.col.Value);
        _ghostBuilding.Name = $"{_buildingId.Value.ToString()}|";
        _ghostBuilding.Visible = false;

        byte duration = ItemDatabase.Get<Building>(_buildingId.Value).Duration;
        Node3D building = _tempBuilding.Instantiate() as Node3D;
        AddChild(building);
        building!.GlobalPosition = _ghostBuilding.GlobalPosition;
        _buildings.Add(((ushort)(currentTurn + duration), new WhileBuilding(_ghostBuilding, building)));

        _buildingId = null;
        _ghostBuilding = null;
    }

    private void SetTransparency(float transparencyValue)
    {
        if (_ghostBuilding is not MeshInstance3D instance3D)
            return;

        instance3D.Transparency = transparencyValue;
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