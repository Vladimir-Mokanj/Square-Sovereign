#if TOOLS
using System.Data;
using FT.Tools;
#endif

using FT.Managers;
using Godot;

namespace FT.Data.Items.Civilization;

public enum BuildingType : byte {NONE, BATTLE, ECONOMY, RESOURCE }

[Tool]
public partial class Building : Item
{
    [Export, ReadOnly] public BuildingType TabType { get; private set; }
    [Export, ReadOnly] public ResourceType ResourceType { get; private set; }
    [Export, ReadOnly] public string Description { get; private set; }
    [Export, ReadOnly] public string Hotkey { get; private set; }
    [Export, ReadOnly] public string Properties { get; private set; }
    [Export] public int[] BuildingProperties { get; private set; }
    [Export] public byte Duration { get; private set; }

#if TOOLS
    public override void Setup(DataRow data)
    {
        base.Setup(data);

        TabType = data.Parse<BuildingType>(nameof(TabType));
        ResourceType = data.Parse<ResourceType>(nameof(ResourceType));
        Description = data.Parse<string>(nameof(Description));
        Hotkey = data.Parse<string>(nameof(Hotkey));
        Properties = data.Parse<string>(nameof(Properties));
        BuildingProperties = SetupProperties(Properties);
        Duration = data.Parse<byte>(nameof(Duration));
    }
#endif
}