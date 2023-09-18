using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class ItemDatabase : Resource
{
    private static List<string> itemDatabase = new();
    public static IReadOnlyList<string> Database => itemDatabase;

    public static void ase()
    {
        GD.Print($"My property value is: {Database.Count}");
    }
}