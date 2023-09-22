using FT.Tools.Observers;

namespace FT.TBS;

public class StateParameters
{
    public ObservableProperty<(byte? row, byte? col)> RowCol { get; } = new((null, null));
    public ObservableProperty<bool> IsMouseClicked { get; } = new(false);
}