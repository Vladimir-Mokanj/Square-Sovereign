using FT.Input;
using Godot;

namespace FT.TBS;

public enum GameState : byte {IDLE, BUILDING, UNIT, ENEMY_TURN} 

public partial class StateController : Node
{
    private StateParameters _stateParameters;

    public override void _Ready()
    {
        IInputController inputController = GetParent().FindChild(nameof(IInputController)) as IInputController;
        inputController?.inputParameters.AddObserver(ControlState);
    }

    private void ControlState(InputDataParameters data)
    {
        _stateParameters.BuildingStateID.Set(data.buildingID);
    }

    public void Initialize(StateParameters stateParameters)
    {
        
        _stateParameters = stateParameters;
        _stateParameters.BuildingStateID.Set(20);
    }
}