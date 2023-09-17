using FT.Data;
using FT.Terrain;
using Godot;

public partial class PlayerController : Node
{
    //[Export] private Camera3D _camera3D;
    //[Export] private TerrainGenerationData _tgd;
//
    //public override void _UnhandledInput(InputEvent @event)
    //{
    //    if (@event is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left } mouseButtonEvent)
    //        return;
//
    //    Vector2 mousePos = mouseButtonEvent.Position;
    //    PhysicsDirectSpaceState3D hit = GetTree().Root.World3D.DirectSpaceState;
    //    PhysicsRayQueryParameters3D _parameters = new();
    //    _parameters.From = _camera3D.ProjectRayOrigin(mousePos);
    //    _parameters.To = _parameters.From + _camera3D.ProjectRayNormal(mousePos) * 500;
//
    //    Godot.Collections.Dictionary results = hit.IntersectRay(_parameters);
//
    //    if (results.Count <= 0)
    //    {
    //        GD.Print("no hit");
    //        return;
    //    }
//
    //    Node collidedNode = (Node)results["collider"];
    //    GenerateTerrain generateTerrain = collidedNode.GetParent<GenerateTerrain>();
    //    if (generateTerrain == null)
    //        return;
    //    
    //    Vector3 hitPoint = (Vector3)results["position"];
    //    byte row = (byte)(hitPoint.X / _tgd.CellSize);
    //    byte col = (byte)(hitPoint.Z / _tgd.CellSize);
    //    
    //    GD.Print("Row: " + row, "Col: " + col);
    //    
    //    GD.Print(generateTerrain._cellManager.GetCellData(row, col));
    //}
}