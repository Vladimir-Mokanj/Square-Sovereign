using FT.Input;
using FT.Managers;
using FT.TBS.States;

namespace FT.TBS;

public class StateController
{
    private (byte row, byte col) _rowCol = (0, 0);
    
    private readonly CellManager _cellManager;
    private InputDataParameters _dataParameters = new();
    public StateParameters StateParameters { get; private set; } = new();
    
    private IGameState _currentGameState;
    public IdleGameState IdleGameState { get; private set; }
    public UnitGameState UnitGameState { get; private set; }
    public BuildingGameState BuildingGameState { get; private set; }
    
    public StateController(IInputController inputController, CellManager cellManager)
    {
        _cellManager = cellManager;
        inputController.inputParameters += ControlState;
        
        IdleGameState = new IdleGameState(inputController, this);
        UnitGameState = new UnitGameState(inputController, this);
        BuildingGameState = new BuildingGameState(inputController, this);

        _currentGameState = IdleGameState;
        _currentGameState.EnterState();
    }

    private void ControlState(InputDataParameters data)
    {
        //StateParameters.RaycastData = _cellManager.GetCellData(data.RowCol);
    }

    public void ChangeState(IGameState newGameState)
    {
        _currentGameState?.ExitState();
        _currentGameState = newGameState;
        _currentGameState?.EnterState();
    }
}