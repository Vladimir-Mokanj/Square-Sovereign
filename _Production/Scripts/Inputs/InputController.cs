using FT.Data;
using FT.TBS;
using Godot;

namespace FT.Input;

public partial class InputController : Node
{
    [Export] private TerrainGenerationData _tgd;
    public StateParameters _stateParameters { get; private set; } = new();

    public override void _Input(InputEvent @event)
    {
        
    }
}