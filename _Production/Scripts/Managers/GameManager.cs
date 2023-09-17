using FT.Data;
using FT.Player;
using FT.Terrain;
using FT.Tools;
using Godot;

namespace FT.Managers;

public partial class GameManager : Node
{
    [Export] private TerrainGenerationData _tgd;
    [Export] private Camera3D _camera;
    
    private CellManager _cellManager;
    private PlayerCustomRaycast _raycast;
    
    public override void _Ready()
    {
        GenerateTerrain generateTerrain = new(ref _tgd);
        generateTerrain.GenerateMesh(ExtentionTools.CreateNode<Node3D>("Terrain", "Terrain", GetTree().GetFirstNodeInGroup("RootNode")));

        _cellManager = new CellManager(_tgd.Rows, _tgd.Cols);
        _cellManager.InitializeCells(_tgd.Rows, generateTerrain.GetYHeights, _tgd.CellSize);

        _raycast = new PlayerCustomRaycast(ref _tgd, _camera);

    }
    
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseButtonEvent)
            return;

        (byte?, byte?) rowCol = _raycast.GetRowCol(mouseButtonEvent.Position);
        if (rowCol.Item1 == null)
            GD.Print("NO SQUARE CLICKED");
        
        GD.Print($"Row: {rowCol.Item1}, Col: {rowCol.Item2}");
        (TerrainType terrainType, ResourceType resourceType, bool isOccupied) cellData = _cellManager.GetCellData(rowCol.Item1!.Value, rowCol.Item2!.Value);
        GD.Print($"Terrain Type: {cellData.terrainType}, Resource Type: {cellData.resourceType}, Is Occupied: {cellData.isOccupied}");
    }
}