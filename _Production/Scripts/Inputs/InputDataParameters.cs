namespace FT.Input;

public class InputDataParameters
{
    public (byte? row, byte? col) rowCol { get; set; }
    public bool isLeftMousePressed { get; set; }
    public bool isRightMousePressed { get; set; }
    public bool areResourcesRevealed { get; set; } = true;
    public bool isMouseDragging { get; set; }
}