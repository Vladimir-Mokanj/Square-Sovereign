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
        _cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetCellMaxYVertexHeight, _tgd.CellSize);

        _raycast = new PlayerCustomRaycast(ref _tgd, _camera, generateTerrain.GetCellMaxYVertexHeight);
    }
    
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseButtonEvent)
            return;

        _currentRowCol = _raycast.GetRowCol(mouseButtonEvent.Position);
        if (!_currentRowCol.row.HasValue || !_currentRowCol.col.HasValue)
        {
            GD.Print("NO SQUARE CLICKED");
            return;
        }

        if (_currentRowCol.row.Value == _oldRowCol.row && _currentRowCol.col.Value == _oldRowCol.col)
        {
            GD.Print($"SAME SQUARE CLICKED: ROW: {_oldRowCol.row}, COL: {_oldRowCol.col}");
            return;
        }

        _oldRowCol = ((byte row, byte col))_currentRowCol;
        _debugTestUi.AssignValues(_cellManager.GetCellData(_oldRowCol.row, _oldRowCol.col));
    }
}