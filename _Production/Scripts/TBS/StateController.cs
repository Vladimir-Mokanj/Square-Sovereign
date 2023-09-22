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
    
    public StateController(TerrainGenerationData tgd, IInputController _inputController)
    {
        _cellManager = new CellManager(tgd.Rows, tgd.Cols);
        
        IdleGameState = new IdleGameState(_inputController, this);
        UnitGameState = new UnitGameState(_inputController, this);
        BuildingGameState = new BuildingGameState(_inputController, this);

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