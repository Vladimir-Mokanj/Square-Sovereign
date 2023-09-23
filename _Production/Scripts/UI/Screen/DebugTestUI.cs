using FT.Managers;
using FT.TBS;
using Godot;

namespace FT.UI;

public partial class DebugTestUI : Node
{
    [Export] private Label _terrainTypeLabel;
    [Export] private Label _resourceTypeLabel;
    [Export] private Label _isOccupiedLabel;

    public override void _Ready() => PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateChanged);
    private void OnStateChanged(StateParameters stateParameters) => stateParameters.RowCol.AddObserver(AssignValues);

    private void AssignValues((byte? row, byte? col) value)
    {
        if (!value.row.HasValue || !value.col.HasValue)
            return;

        UnpackedCellData data = CellManager.GetCellData((value.row.Value, value.col.Value));

        _terrainTypeLabel.Text = $"Terrain Type: {data.terrainType}";
        _resourceTypeLabel.Text = $"Resource Type: {data.resourceType}";
        _isOccupiedLabel.Text = data.isOccupied ? "Is Occupied: True" : "Is Occupied: False";
    }
}
