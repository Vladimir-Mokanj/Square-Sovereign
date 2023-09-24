using System.Data;
using FT.Tools;
using Godot;

namespace FT.Data.Items;

[Tool]
public partial class Unit : Item
{
    [Export, ReadOnly] public string Description { get; private set; }

#if TOOLS
    public override void Setup(DataRow data)
    {
        base.Setup(data);
        
        Description = data.Parse<string>(nameof(Description));
    }
#endif
}
