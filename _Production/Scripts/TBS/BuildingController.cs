using Godot;

namespace FT.TBS;

public partial class BuildingController : Node
{
    private int buildingId = -1;
    
    public override void _Ready() => 
        GetParent<PlayerManager>().OnStateInitialized.AddObserver(State);

    private void State(StateParameters State) => 
        State.BuildingStateID.AddObserver(id => buildingId = id);
}