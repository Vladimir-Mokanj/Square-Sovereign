using FT.Data;
using FT.Input;
using FT.Managers;
using FT.TBS.States;

namespace FT.TBS;

public class StateController
{
    private readonly CellManager _cellManager;
    private InputDataParameters _dataParameters = new();
    
    private IGameState _currentGameState;
    public IdleGameState IdleGameState { get; private set; }
    public UnitGameState UnitGameState { get; private set; }
    public BuildingGameState BuildingGameState { get; private set; }
    
    public StateController(IInputController inputController, CellManager cellManager)
    {
        _cellManager = cellManager;
        
        IdleGameState = new IdleGameState(inputController, this);
        UnitGameState = new UnitGameState(inputController, this);
        BuildingGameState = new BuildingGameState(inputController, this);

        _currentGameState = IdleGameState;
        _currentGameState.EnterState();
    }
    
    public void ChangeState(IGameState newGameState)
    {
        _currentGameState?.ExitState();
        _currentGameState = newGameState;
        _currentGameState?.EnterState();
    }
}