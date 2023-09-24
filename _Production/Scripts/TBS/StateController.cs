using FT.Input;
using FT.Player;
using Godot;

namespace FT.TBS;

public partial class StateController : Node
{
    private StateParameters _state;

    public override void _Ready()
    {
        IInputController inputController = GetParent().FindChild(nameof(InputController)) as IInputController;
        inputController?.inputParameters.AddObserver(ControlState);
    }

    private void ControlState(InputDataParameters data)
    {
        _state.IsMouseLeftDown.Set(data.isLeftMousePressed);
        _state.IsMouseRightDown.Set(data.isRightMousePressed);
        _state.RowCol.Set(PlayerCustomRaycast.GetRowCol(data.mousePosition));
    }

    public void Initialize(StateParameters stateParameters) => 
        _state = stateParameters;
}