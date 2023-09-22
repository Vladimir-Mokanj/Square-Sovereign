using FT.Managers;
using Godot;

namespace FT.UI;

public partial class DebugTestUI : Node
{
    [Export] private Label _terrainTypeLabel;
    [Export] private Label _resourceTypeLabel;
    [Export] private Label _isOccupiedLabel;

    public void AssignValues((TerrainType terrainType, ResourceType resourceType, bool isOccupied) cellData)
    {
        _terrainTypeLabel.Text = $"Terrain Type: {cellData.terrainType}";
        _resourceTypeLabel.Text = $"Resource Type: {cellData.resourceType}";
        _isOccupiedLabel.Text = cellData.isOccupied ? "Is Occupied: True" : "Is Occupied: False";
    }
}
