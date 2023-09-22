#if TOOLS
using System.Data;
using FT.Tools;
#endif

using Godot;

namespace FT.Data;

public partial class Item : ItemBase
{
    [Export, ReadOnly] public string DisplayName { get; private set; }
    [Export, ReadOnly] public string PrefabName { get; private set; }
    [Export, ReadOnly] public PackedScene Prefab { get; private set; }
    [Export, ReadOnly] public string SpriteName { get; private set; }
    [Export, ReadOnly] public Texture2D Sprite { get; private set; }

#if TOOLS
    public virtual void Setup(DataRow data)
    {
        Name = data.Parse<string>(nameof(Name));
        DisplayName = data.Parse<string>(nameof(DisplayName));
        PrefabName = data.Parse<string>(nameof(PrefabName));
        Prefab = GD.Load<PackedScene>($"res://Resources/Items/{GetType().Name}/Prefabs/{PrefabName}.tscn");
        SpriteName = data.Parse<string>(nameof(SpriteName));
        Sprite = GD.Load<Texture2D>($"res://Resources/Items/{GetType().Name}/Icons/{SpriteName}.png");
    }
#endif
}
