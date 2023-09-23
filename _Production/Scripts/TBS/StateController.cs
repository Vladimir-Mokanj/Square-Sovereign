using FT.Input;
using FT.Player;
using Godot;

namespace FT.TBS;

public partial class StateController : Node
{
    private StateParameters _stateParameters;

    public override void _Ready()
    {
        IInputController inputController = GetParent().FindChild(nameof(InputController)) as IInputController;
        inputController?.inputParameters.AddObserver(ControlState);
    }

    private void ControlState(InputDataParameters data)
    {
        _stateParameters.IsMouseLeftDown.Set(data.isLeftMousePressed);
        _stateParameters.IsMouseRightDown.Set(data.isRightMousePressed);
        _stateParameters.RowCol.Set(PlayerCustomRaycast.GetRowCol(data.mousePosition));
    }

    public void Initialize(StateParameters stateParameters) => 
        _stateParameters = stateParameters;
}