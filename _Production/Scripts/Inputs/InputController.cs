using System;
using FT.Data;
using FT.Player;
using Godot;

namespace FT.Input;

public partial class InputController : Node, IInputController
{
    public Action<InputDataParameters> inputParameters { get; set; }
    
    public InputDataParameters _dataParameters { get; private set; } = new();
    private PlayerCustomRaycast _raycast;

    public void Initialize(ref TerrainGenerationData tgd, byte[] yHeights ) => 
        _raycast = new PlayerCustomRaycast(ref tgd, GetTree().GetFirstNodeInGroup("MainCamera") as Camera3D, yHeights);
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            _dataParameters.isMousePressed = true;

        if (@event is InputEventMouseMotion mouseMotionEvent)
            _dataParameters.RowCol = _raycast.GetRowCol(mouseMotionEvent.Position);
    }
    
    public override void _Process(double delta)
    {
        inputParameters?.Invoke(_dataParameters);
        _dataParameters.isMousePressed = false;
    }
}

public interface IInputController
{
    public Action<InputDataParameters> inputParameters { get; set; }
}