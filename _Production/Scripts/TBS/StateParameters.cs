using FT.Tools.Observers;

namespace FT.TBS;

public class StateParameters
{
    public ObservableProperty<bool> IsMouseClicked { get; } = new(false);
}