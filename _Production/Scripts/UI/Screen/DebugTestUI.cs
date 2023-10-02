using FT.Data;
using FT.Managers;
using FT.TBS;
using Godot;

namespace FT.UI;

public partial class DebugTestUI : Node
{
    [Export] private Label _terrainTypeLabel;
    [Export] private Label _resourceTypeLabel;
    [Export] private Label _isOccupiedLabel;

    [Export] private Button _englandButton;
    [Export] private Button _franceButton;
    
    
    [Export] private Material _civColor;
    
    public override void _Ready()
    {
        PlayerManager.Instance.OnStateInitialized.AddObserver(OnStateAssigned);

        _englandButton.Pressed += () =>
        {
            ItemDatabase.SetCivDebug("England");
            ((BuildingController)GetTree().GetFirstNodeInGroup(nameof(BuildingController)))?.InitializeBuildScreen();
            _civColor.Set("albedo_color", new Color(0.67f, 0.152f, 0.114f));
        };
        _franceButton.Pressed += () =>
        {
            ItemDatabase.SetCivDebug("France");
            ((BuildingController)GetTree().GetFirstNodeInGroup(nameof(BuildingController)))?.InitializeBuildScreen();
            _civColor.Set("albedo_color", new Color(0.294f, 0.576f, 0.721f));
        };
    }

    private void OnStateAssigned(StateParameters State)
    {
        State.RowCol.AddObserver(AssignValues);
        State.IsMouseLeftDown.AddObserver(value => { if (value) AssignValues(State.RowCol); });
    }

    private void AssignValues((byte? row, byte? col) value)
    {
        bool isNull = value is { row: not null, col: not null };
        UnpackedCellData data = isNull ? CellManager.GetCellData((value.row.Value, value.col.Value)) : new UnpackedCellData();

        _terrainTypeLabel.Text = $"Terrain Type: {(isNull ? data.terrainType : "Null")}";
        _resourceTypeLabel.Text = $"Resource Type: {(isNull ? data.resourceType : "Null")}";
        _isOccupiedLabel.Text = isNull ? (data.isOccupied ? "Is Occupied: True" : "Is Occupied: False") : "Is Occupied: Null";
    }
    
}
