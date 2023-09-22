using System;
using FT.Data;
using FT.Input;
using FT.Managers;
using FT.Terrain;
using FT.Tools;
using FT.UI;
using Godot;

namespace FT.TBS;

public partial class GameManager : Node
{
    [Export] private TerrainGenerationData _tgd;
    [Export] private InputController _inputController;
    [Export] private BuildingScreen _temp;

    [Export] private DebugTestUI _debugTestUi;
    
    public static GameManager Instance { get; private set; }
    public Action<StateParameters> OnGameInitialized { get; set; }

    private StateController _stateController;
    
    public override void _Ready()
    {
        Instance = this;
        
        GenerateTerrain generateTerrain = new(_tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        CellManager cellManager = new(_tgd.Rows, _tgd.Cols);
        cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight);
        
        _inputController.Initialize(ref _tgd, generateTerrain.GetCellMaxYVertexHeight);
        _stateController = new StateController(_inputController, cellManager);
    }

    private void GameInitialized()
    {
        if (_stateController == null)
            return;
        
        OnGameInitialized?.Invoke(_stateController.StateParameters);
    }
    
    //public override void _Input(InputEvent @event)
    //{
        //if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        //{
        //    if (!_temp.BuildStructure(_oldRowCol.row, _oldRowCol.col, _tgd.CellSize, _cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col)))
        //        return;
        //    
        //    _cellManager.SetIsOccupied(_oldRowCol.row, _oldRowCol.col);
        //    _debugTestUi.AssignValues(_cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col));
        //}
//
        //if (@event is InputEventMouseMotion mouseMotionEvent)
        //{
        //    _currentRowCol = _raycast.GetRowCol(mouseMotionEvent.Position);
        //    if (!_currentRowCol.row.HasValue || !_currentRowCol.col.HasValue)
        //        return;
        //
        //    if (_currentRowCol.row.Value == _oldRowCol.row && _currentRowCol.col.Value == _oldRowCol.col)
        //        return;
//
        //    _oldRowCol = ((byte row, byte col))_currentRowCol;
        //    _debugTestUi.AssignValues(_cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col));
        //}
    //}
}