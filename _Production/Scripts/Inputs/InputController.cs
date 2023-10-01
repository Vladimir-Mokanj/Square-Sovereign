using System;
using FT.Player;
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
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Right }) 
            _dataParameters.isMouseDragging = true;

        if (@event is InputEventMouseButton { Pressed: false, ButtonIndex: MouseButton.Right })
            _dataParameters.isMouseDragging = false;

        
        if (@event.IsActionPressed("OnResourcesRevealed"))
            _dataParameters.areResourcesRevealed = !_dataParameters.areResourcesRevealed;

        if (@event is InputEventMouseMotion mouseMotionEvent)
            _dataParameters.rowCol = PlayerCustomRaycast.GetRowCol(mouseMotionEvent.GlobalPosition);
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