using System;
using FT.Input;

namespace FT.TBS.States;

public enum GameState : byte {GENERATE, IDLE, BUILDING, UNIT, END}

public abstract class GameStateBase : IGameState
{
    public Action<GameState> OnEnterState { get; set; }
    public Action<GameState> OnExitState { get; set; }
    private readonly IInputController _inputController;
    private readonly GameState _gameState;
    
    protected readonly StateController _stateController;
    
    protected GameStateBase(IInputController inputController, StateController stateController, GameState gameState)
    {
        _inputController = inputController;
        _stateController = stateController;
        _gameState = gameState;
    }

    protected abstract void OnInput(InputDataParameters data);

    public virtual void EnterState()
    {
        OnEnterState?.Invoke(_gameState);
        _inputController.inputParameters += OnInput;
    }

    public virtual void ExitState()
    {
        OnExitState?.Invoke(_gameState);
        _inputController.inputParameters -= OnInput;
    }
}