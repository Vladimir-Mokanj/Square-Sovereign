using FT.Input;
using Godot;

namespace FT.TBS;

public enum GameState : byte {IDLE, BUILDING, UNIT, ENEMY_TURN} 

public partial class StateController : Node
{
    public StateParameters StateParameters { get; private set; } = new();

    public override void _Ready()
    {
        IInputController inputController = GetParent().FindChild(nameof(IInputController)) as IInputController;
        inputController?.inputParameters.AddObserver(ControlState);
    }

    private void ControlState(InputDataParameters data)
    {
        StateParameters.BuildingStateID.Set(data.buildingID);
    }
}