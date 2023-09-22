using FT.Input;

namespace FT.TBS.States;

public class BuildingGameState : GameStateBase
{
    public BuildingGameState(IInputController onInput, StateController stateController) : base(onInput, stateController, GameState.BUILDING) { }
    
    protected override void OnInput(InputDataParameters data)
    {

    }
}