using System;
using FT.Tools.Observers;
using Godot;

namespace FT.Input;

public partial class InputController : Node, IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters => _inputParameters;
    private readonly ObservableAction<Action<InputDataParameters>> _inputParameters = new();

    private readonly InputDataParameters _dataParameters = new();

    public override void _Input(InputEvent @event)
    {
        _dataParameters.isMousePressed = @event.IsActionPressed("OnPressLeft");
        
        if (@event is InputEventMouseMotion mouseMotionEvent)
            _dataParameters.MousePosition = mouseMotionEvent.Position;
    }

    public override void _Process(double delta) => 
        _inputParameters.Action?.Invoke(_dataParameters);
}

public interface IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters { get; }
}