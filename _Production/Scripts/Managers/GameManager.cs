using FT.Data;
using FT.Player;
using FT.Terrain;
using FT.Tools;
using FT.UI;
using Godot;

namespace FT.Managers;

public partial class GameManager : Node
{
    [Export] private TerrainGenerationData _tgd;
    [Export] private Camera3D _camera;
    [Export] private BuildingScreen _temp;
    
    private CellManager _cellManager;
    private PlayerCustomRaycast _raycast;

    [Export] private DebugTestUI _debugTestUi;
    private (byte? row, byte? col) _currentRowCol = (null, null);
    private (byte row, byte col) _oldRowCol = (0, 0);
    
    public override void _Ready()
    {
        GenerateTerrain generateTerrain = new(ref _tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        _cellManager = new CellManager(_tgd.Rows, _tgd.Cols);
        _cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight);

        _raycast = new PlayerCustomRaycast(ref _tgd, _camera, generateTerrain.GetCellMaxYVertexHeight);
    }
    
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
        {
            if (!_temp.BuildStructure(_oldRowCol.row, _oldRowCol.col, _tgd.CellSize, _cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col)))
                return;
            
            _cellManager.SetIsOccupied(_oldRowCol.row, _oldRowCol.col);
            _debugTestUi.AssignValues(_cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col));
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            _currentRowCol = _raycast.GetRowCol(mouseMotionEvent.Position);
            if (!_currentRowCol.row.HasValue || !_currentRowCol.col.HasValue)
                return;
        
            if (_currentRowCol.row.Value == _oldRowCol.row && _currentRowCol.col.Value == _oldRowCol.col)
                return;

            _oldRowCol = ((byte row, byte col))_currentRowCol;
            _debugTestUi.AssignValues(_cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col));
        }
    }
}