using FT.Input;
using FT.Managers;
using FT.TBS.States;

namespace FT.TBS;

public class StateController
{
    private readonly CellManager _cellManager;
    public StateParameters StateParameters { get; private set; } = new();
    
    private IGameState _currentGameState;
    public IdleGameState IdleGameState { get; private set; }
    public UnitGameState UnitGameState { get; private set; }
    public BuildingGameState BuildingGameState { get; private set; }
    
    public StateController(IInputController inputController, CellManager cellManager)
    {
        _cellManager = cellManager;
        inputController.inputParameters += ControlState;
        
        IdleGameState = new IdleGameState(StateParameters);
        UnitGameState = new UnitGameState(StateParameters);
        BuildingGameState = new BuildingGameState(StateParameters);

        _currentGameState = IdleGameState;
        _currentGameState.EnterState();
    }

    private void ControlState(InputDataParameters data)
    {
        StateParameters.RaycastData.Set(_cellManager.GetCellData(data.RowCol));
    }

    public void ChangeState(IGameState newGameState)
    {
        _currentGameState?.ExitState();
        _currentGameState = newGameState;
        _currentGameState?.EnterState();
    }
}