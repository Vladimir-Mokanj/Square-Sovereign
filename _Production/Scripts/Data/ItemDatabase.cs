using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace FT.Data;

[Tool]
public partial class ItemDatabase : ItemDatabaseBase<Item, ItemDatabase>
{
    public string GetDownloadUrl(Type type)
    {
        throw new NotImplementedException();
    }

    public static List<Type> GetAllItemTypes() => 
        typeof(Item).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Item))).ToList();
}