using System;

namespace FT.TBS.States;

public enum GameState : byte {GENERATE, IDLE, BUILDING, UNIT, END}

public abstract class GameStateBase : IGameState
{
    public Action<GameState> OnEnterState { get; set; }
    public Action<GameState> OnExitState { get; set; }
    private readonly GameState _gameState;

    protected GameStateBase(StateParameters state, GameState gameState)
    {
        _gameState = gameState;
        state.GameState.AddObserver(ChangeState);
    }

    private void ChangeState(GameState gameState)
    {
        if (gameState == _gameState)
            EnterState();
        ExitState();
    }

    public virtual void EnterState()
    {
        OnEnterState?.Invoke(_gameState);
    }

    public virtual void ExitState()
    {
        OnExitState?.Invoke(_gameState);
    }
}