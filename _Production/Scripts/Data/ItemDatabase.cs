using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FT.Data;

[Tool]
public partial class ItemDatabase : ItemDatabaseBase<Item, ItemDatabase>
{
    public string GetDownloadUrl(Type type) => 
        $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/gviz/tq?tqx=out:csv&sheet={type.Name}";

    public static IEnumerable<Type> GetAllItemTypes() => 
        typeof(Item).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Item))).ToList();
}