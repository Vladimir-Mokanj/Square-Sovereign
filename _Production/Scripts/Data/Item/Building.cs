#if TOOLS
using System.Data;
using FT.Tools;
#endif

using Godot;

namespace FT.Data.Items;

public enum BuildingType : byte {ECONOMY, RESOURCE, BATTLE}

[Tool]
public partial class Building : Item
{
    [Export, ReadOnly] public BuildingType TabType { get; private set; }

#if TOOLS
    public override void Setup(DataRow data)
    {
        base.Setup(data);

        TabType = data.Parse<BuildingType>(nameof(TabType));
    }
#endif
}