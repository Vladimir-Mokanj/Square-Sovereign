using System;

namespace FT.TBS.States;

public enum GameState : byte {GENERATE, IDLE, BUILDING, UNIT, END}

public abstract class GameStateBase : IGameState
{
    public Action<GameState> OnEnterState { get; set; }
    public Action<GameState> OnExitState { get; set; }
    private readonly GameState _gameState;
    protected readonly StateParameters _stateParameters;

    protected GameStateBase(StateParameters stateParameters, GameState gameState)
    {
        _stateParameters = stateParameters;
        _gameState = gameState;
    }

    public virtual void EnterState() => OnEnterState?.Invoke(_gameState);
    public virtual void ExitState() => OnExitState?.Invoke(_gameState);
}