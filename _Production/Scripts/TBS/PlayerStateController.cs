using FT.TBS.States;
using Godot;

namespace FT.TBS;

public partial class PlayerStateController : Node
{
    private readonly StateParameters _stateParameters = new();
    
    private IGameState _currentGameState;
    private IdleGameState _idleGameState;
    private UnitGameState _unitGameState;
    private BuildingGameState _buildingGameState;
    
    public override void _Ready()
    {
        _idleGameState = new IdleGameState(_stateParameters);
        _unitGameState = new UnitGameState(_stateParameters);
        _buildingGameState = new BuildingGameState(_stateParameters);
    }

    public void ChangeState(IGameState newGameState)
    {
        _currentGameState?.ExitState();
        _currentGameState = newGameState;
        _currentGameState?.EnterState();
    }
}