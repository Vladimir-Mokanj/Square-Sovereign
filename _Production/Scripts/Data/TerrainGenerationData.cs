using Godot;

namespace FT.Data;

[GlobalClass]
public partial class TerrainGenerationData : Resource
{
    [ExportCategory("Textures/Shader")]
    [Export] public Shader Shader { get; private set; }
    [Export] public CompressedTexture2D WaterTexture { get; private set; }
    [Export] public CompressedTexture2D DirtTexture { get; private set; }
    [Export] public CompressedTexture2D StoneTexture { get; private set; }
    
    [ExportCategory("Terrain Data")]
    [Export] public int Seed { get; private set; } = 123456789;
    [Export] public byte Rows { get; private set; } = 20;
    [Export] public byte Cols { get; private set; } = 20;
}