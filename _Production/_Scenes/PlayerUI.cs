using Godot;

namespace FT.UI;

public partial class PlayerUI : Node
{
    #if TOOLS
    [Export] public DebugTestUI DebugTestUi { get; private set; }
    #endif
    
    [Export] public BuildingScreen BuildingScreen { get; private set; }
}