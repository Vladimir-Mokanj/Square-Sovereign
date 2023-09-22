using FT.Input;

namespace FT.TBS.States;

public class IdleGameState : GameStateBase
{
    public IdleGameState(IInputController stateParameters, StateController stateController) : base(stateParameters, stateController, GameState.IDLE) { }
    
    protected override void OnInput(InputDataParameters data)
    {

    }
}