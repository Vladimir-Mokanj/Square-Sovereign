#if TOOLS
using System.Data;
using FT.Tools;
#endif

using Godot;

namespace FT.Data;

public partial class Item : ItemBase
{
    [Export] public string DisplayName { get; private set; }
    [Export] public string PrefabName { get; private set; }
    [Export] public PackedScene Prefab { get; private set; }
    [Export] public string SpriteName { get; private set; }
    [Export] public Texture Sprite { get; private set; }
    [Export] public int MaxStackAmount { get; private set; }

#if TOOLS
    public virtual void Setup(DataRow data)
    {
        Name = data.Parse<string>(nameof(Name));
        DisplayName = data.Parse<string>(nameof(DisplayName));
        PrefabName = data.Parse<string>(nameof(PrefabName));
        SpriteName = data.Parse<string>(nameof(SpriteName));
        Sprite = GD.Load<Texture>($"res://Resources/Items/{GetType().Name}/Icons/{SpriteName}.png");
        MaxStackAmount = data.Parse<int>(nameof(MaxStackAmount));
        Prefab = GD.Load<PackedScene>($"res://Resources/Items/{GetType().Name}/Prefabs/{PrefabName}.tscn");
    }
#endif
}
