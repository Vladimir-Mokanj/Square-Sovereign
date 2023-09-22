using FT.Managers;
using FT.TBS.States;
using FT.Tools.Observers;

namespace FT.TBS;

public class StateParameters
{
    public ObservableProperty<(byte row, byte col)> RowCol { get; } = new();
    public ObservableProperty<GameState> GameState { get; } = new();
    public ObservableProperty<(TerrainType, ResourceType, bool)?> RaycastData { get; } = new();
}