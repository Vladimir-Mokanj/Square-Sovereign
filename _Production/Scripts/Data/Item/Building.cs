#if TOOLS
using System.Data;
using FT.Tools;
#endif

using FT.Managers;
using Godot;

namespace FT.Data.Items;

public enum BuildingType : byte {ECONOMY, BATTLE, RESOURCE}

[Tool]
public partial class Building : Item
{
    [Export, ReadOnly] public BuildingType TabType { get; private set; }
    [Export, ReadOnly] public ResourceType ResourceType { get; private set; }

#if TOOLS
    public override void Setup(DataRow data)
    {
        base.Setup(data);

        TabType = data.Parse<BuildingType>(nameof(TabType));
        ResourceType = data.Parse<ResourceType>(nameof(ResourceType));
    }
#endif
}