using FT.Managers;
using FT.TBS;
using Godot;

namespace FT.UI;

public partial class DebugTestUI : Node
{
    [Export] private Label _terrainTypeLabel;
    [Export] private Label _resourceTypeLabel;
    [Export] private Label _isOccupiedLabel;

    //public override void _Ready() => GameManager.Instance.OnGameInitialized += OnStateChanged;
    //private void OnStateChanged(StateParameters stateParameters) => stateParameters.RaycastData.AddObserver(AssignValues);

    private void AssignValues((TerrainType terrainType, ResourceType resourceType, bool isOccupied)? cellData)
    {
        _terrainTypeLabel.Text = $"Terrain Type: {cellData.Value.terrainType}";
        _resourceTypeLabel.Text = $"Resource Type: {cellData.Value.resourceType}";
        _isOccupiedLabel.Text = cellData.Value.isOccupied ? "Is Occupied: True" : "Is Occupied: False";
    }
}
