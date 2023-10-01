using FT.Tools.Observers;

namespace FT.TBS;

public class StateParameters
{
    public ObservableProperty<(byte? row, byte? col)> RowCol { get; } = new((null, null));
    public ObservableProperty<int?> BuildingSelectedID { get; } = new(null);
    public ObservableProperty<bool> IsMouseLeftDown { get; } = new(false);
    public ObservableProperty<bool> IsMouseRightDown { get; } = new(false);
    public ObservableProperty<bool> AreResourcesRevealed { get; } = new(true);
    public ObservableProperty<bool> IsMouseDrag { get; } = new(false);
}