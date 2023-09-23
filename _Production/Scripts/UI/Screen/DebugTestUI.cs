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
        UnpackedCellData data = new();
        bool isNull = value is { row: not null, col: not null };
        if (isNull)
            data = CellManager.GetCellData((value.row.Value, value.col.Value));

        _terrainTypeLabel.Text = $"Terrain Type: {(isNull ? data.terrainType : "Null")}";
        _resourceTypeLabel.Text = $"Resource Type: {(isNull ? data.resourceType : "Null")}";
        _isOccupiedLabel.Text = isNull ? data.isOccupied ? "Is Occupied: True" : "Is Occupied: False" : "Is Occupied: Null";
    }
}
