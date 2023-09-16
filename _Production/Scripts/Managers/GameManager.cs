using FT.Data;
using FT.Terrain;
using FT.Tools;
using Godot;

namespace FT.Managers;

public partial class GameManager : Node
{
    [Export] private TerrainGenerationData _tgd;
    
    public override void _Ready()
    {
        new GenerateTerrain(_tgd).GenerateMesh(ExtensionTools.CreateNode<Node3D>("Terrain", "Terrain", this), _tgd);
    }
}