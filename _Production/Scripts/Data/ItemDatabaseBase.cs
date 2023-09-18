using Godot;

namespace FT.Data;

[GlobalClass]
public partial class ItemDatabaseBase<T, TI> : Resource where T : ItemBase where TI : ItemDatabaseBase<T, TI>
{
    [Export] protected Item[] _items;

    public static TI Database => _database ??= GD.Load("Resources/" + typeof(TI).Name + ".tres") as TI;
    private static TI _database;

    public static void ase()
    {
        GD.Print($"My property value is: {Database._items.Length}");
    }
}