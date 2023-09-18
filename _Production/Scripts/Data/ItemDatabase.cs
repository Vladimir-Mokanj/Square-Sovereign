using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FT.Data;

[Tool]
public partial class ItemDatabase : ItemDatabaseBase<Item, ItemDatabase>
{
    public string GetDownloadUrl(Type type) => 
        $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/gviz/tq?tqx=out:csv&sheet={type.Name}&tq=select%20*%20where%20A%3D%22{type.Name}%22%20or%20A%3D%22Type%22";

    public static List<Type> GetAllItemTypes() => 
        typeof(Item).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Item))).ToList();
}