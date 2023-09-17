using FT.Data;
using FT.Terrain;
using FT.Tools;
using Godot;

namespace FT.Managers;

public partial class GameManager : Node
{
    [Export] private TerrainGenerationData _tgd;

    private CellManager _cellManager;
    
    public override void _Ready()
    {
        GenerateTerrain generateTerrain = new(ref _tgd);
        generateTerrain.GenerateMesh(ExtensionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        _cellManager = new CellManager(_tgd.Rows, _tgd.Cols);
        _cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetYHeights, _tgd.CellSize);

    }
}