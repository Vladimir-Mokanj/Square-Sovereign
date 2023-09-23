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
        _stateParameters.IsMouseClicked.Set(data.isMousePressed);
        _stateParameters.RowCol.Set(PlayerCustomRaycast.GetRowCol(data.MousePosition));
    }

    public void Initialize(StateParameters stateParameters) => 
        _stateParameters = stateParameters;
}