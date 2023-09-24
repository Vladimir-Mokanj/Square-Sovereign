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
        if (@event is InputEventMouseMotion mouseMotionEvent)
            _dataParameters.mousePosition = mouseMotionEvent.Position;
    }

    public override void _Process(double delta)
    {
        _dataParameters.isLeftMousePressed = Godot.Input.IsActionJustPressed("OnPressLeft");
        _dataParameters.isRightMousePressed = Godot.Input.IsActionJustPressed("OnPressRight");
        _inputParameters.Action?.Invoke(_dataParameters);
    }
}

public interface IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters { get; }
}