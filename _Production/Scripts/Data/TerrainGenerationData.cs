using Godot;

namespace FT.Data;

[GlobalClass]
public partial class TerrainGenerationData : Resource
{
    [Export] public int Seed { get; private set; } = 123456789;
    [Export] public ushort CellSize { get; private set; } = 5;
    [Export] public ushort Rows { get; private set; } = 20;
    [Export] public ushort Cols { get; private set; } = 20;
}