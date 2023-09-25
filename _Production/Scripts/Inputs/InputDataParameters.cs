using Godot;

namespace FT.Input;

public class InputDataParameters
{
    public Vector2 mousePosition { get; set; }
    public bool isLeftMousePressed { get; set; }
    public bool isRightMousePressed { get; set; }
    public bool areResourcesRevealed { get; set; } = true;
}