using FT.Input;
using FT.TBS.States;
using Godot;

namespace FT.TBS;

public partial class StateController : Node
{
    private InputController _inputController;
    
    private IGameState _currentGameState;
    private IdleGameState _idleGameState;
    private UnitGameState _unitGameState;
    private BuildingGameState _buildingGameState;
    
    public override void _Ready()
    {
        _inputController = GetParent().GetNode<InputController>(nameof(InputController));

        _idleGameState = new IdleGameState(_inputController._stateParameters);
        _unitGameState = new UnitGameState(_inputController._stateParameters);
        _buildingGameState = new BuildingGameState(_inputController._stateParameters);
    }

    public void ChangeState(IGameState newGameState)
    {
        _currentGameState?.ExitState();
        _currentGameState = newGameState;
        _currentGameState?.EnterState();
    }
}