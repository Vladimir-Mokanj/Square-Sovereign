using System.Linq;
using FT.Input;
using FT.UI;
using Godot;
using Godot.Collections;

namespace FT.TBS;

public partial class StateController : Node3D
{
    private StateParameters _state;

    public override void _Ready()
    {
        IInputController inputController = GetParent().FindChild(nameof(InputController)) as IInputController;
        inputController?.inputParameters.AddObserver(ControlState);
    }

    private void ControlState(InputDataParameters data)
    {
        bool isOverButton = IsButtonPressed();
        
        if (data.isLeftMousePressed && !isOverButton) _state.BuildingSelectedID.Set(PhysicsRaycast(_state.BuildingSelectedID.Value));
        if (data.isRightMousePressed) _state.BuildingSelectedID.Set(null);
        if (!isOverButton) _state.IsMouseLeftDown.Set(data.isLeftMousePressed);
        _state.IsMouseRightDown.Set(data.isRightMousePressed);
        _state.RowCol.Set(data.rowCol);
        _state.AreResourcesRevealed.Set(data.areResourcesRevealed);
        _state.IsMouseDrag.Set(data.isMouseDragging);
    }

    public void Initialize(StateParameters stateParameters) => 
        _state = stateParameters;

    private bool IsButtonPressed() => 
        GetViewport().GuiGetFocusOwner() is DisplayUI control && control.GetGlobalRect().HasPoint(GetViewport().GetMousePosition());

    private int? PhysicsRaycast(int? value)
    {
        Camera3D camera = GetViewport().GetCamera3D();
        Vector3 rayTo = camera.ProjectRayOrigin(GetViewport().GetMousePosition()) + camera.ProjectRayNormal(GetViewport().GetMousePosition()) * 1000.0f;

        PhysicsDirectSpaceState3D spaceState = PhysicsServer3D.SpaceGetDirectState(GetWorld3D().Space);
        Dictionary hitResult = spaceState.IntersectRay(PhysicsRayQueryParameters3D.Create(camera.GlobalPosition, rayTo));

        if (hitResult.Count <= 0)
            return value;

        Node parent = ((Node)hitResult["collider"]).GetParent();
        if (parent != null && int.TryParse(parent.Name.ToString().TakeWhile(c => c != '|').ToArray(), out int objectId))
            return objectId;

        return value;
    }
}