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

    public override void _Process(double delta) => 
        _inputParameters?.Action.Invoke(_dataParameters);
}

public interface IInputController
{
    public IObservableAction<Action<InputDataParameters>> inputParameters { get; }
}