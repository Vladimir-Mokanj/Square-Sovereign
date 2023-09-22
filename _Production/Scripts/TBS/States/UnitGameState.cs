using FT.Input;

namespace FT.TBS.States;

public class UnitGameState : GameStateBase
{
    public UnitGameState(IInputController stateParameters, StateController stateController) : base(stateParameters, stateController, GameState.UNIT) { }
    
    protected override void OnInput(InputDataParameters data)
    {

    }
}