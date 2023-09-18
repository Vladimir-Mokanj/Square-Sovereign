using Godot;
using System.Data;
using FT.Tools;

namespace FT.Data;

public partial class Item : ItemBase
{
    [Export] public string DisplayName { get; private set; }
    [Export] public string PrefabName { get; private set; }
    [Export] public string SpriteName { get; private set; }
    [Export] public int MaxStackAmount { get; private set; }
    
    public virtual void Setup(DataRow data)
    {
        Name = data.Parse<string>(nameof(Name));
        DisplayName = data.Parse<string>(nameof(DisplayName));
        PrefabName = data.Parse<string>(nameof(PrefabName));
        SpriteName = data.Parse<string>(nameof(SpriteName));
        MaxStackAmount = data.Parse<int>(nameof(MaxStackAmount));
    }
}
