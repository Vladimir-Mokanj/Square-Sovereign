using System.Linq;
using FT.Input;
using FT.Player;
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
        bool isOverButton = IsButtonPressed(data.mousePosition);
        
        if (data.isLeftMousePressed && !isOverButton) _state.BuildingSelectedID.Set(PhysicsRaycast(data.mousePosition, _state.BuildingSelectedID.Value));
        if (data.isRightMousePressed) _state.BuildingSelectedID.Set(null);
        if (!isOverButton) _state.IsMouseLeftDown.Set(data.isLeftMousePressed);
        _state.IsMouseRightDown.Set(data.isRightMousePressed);
        _state.RowCol.Set(PlayerCustomRaycast.GetRowCol(data.mousePosition));
        _state.AreResourcesRevealed.Set(data.areResourcesRevealed);
    }

    public void Initialize(StateParameters stateParameters) => 
        _state = stateParameters;

    private bool IsButtonPressed(Vector2 mousePosition) => 
        GetViewport().GuiGetFocusOwner() is DisplayUI control && control.GetGlobalRect().HasPoint(mousePosition);

    private int? PhysicsRaycast(Vector2 mousePosition, int? value)
    {
        Camera3D camera = GetViewport().GetCamera3D();
        Vector3 rayTo = camera.ProjectRayOrigin(mousePosition) + camera.ProjectRayNormal(mousePosition) * 1000.0f;

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