using FT.Tools.Observers;

namespace FT.TBS;

public class StateParameters
{
    public ObservableProperty<int> BuildingStateID { get; } = new(-1);
}