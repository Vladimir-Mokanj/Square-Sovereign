using System;
using FT.Tools.Observers;
using Godot;

namespace FT.Input;

public partial class InputController : Node, IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters => _inputParameters;
    private readonly ObservableAction<Action<InputDataParameters>> _inputParameters = new();

    private InputDataParameters _dataParameters;
    
    public void Initialize(InputDataParameters dataParameters) =>
        _dataParameters = dataParameters;

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            _dataParameters.isMousePressed = true;
    }

    public override void _Process(double delta)
    {
        _inputParameters.Action?.Invoke(_dataParameters);
        _dataParameters.isMousePressed = false;
        _dataParameters.buildingID = -1;
    }
}

public interface IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters { get; }
}