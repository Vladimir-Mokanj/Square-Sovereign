using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FT.Tools;
using Godot;

namespace FT.Data;

[GlobalClass, Tool]
public partial class ItemDatabase : ItemDatabaseBase<Item, ItemDatabase>
{
    public string GetDownloadUrl(Type type) => 
        $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/gviz/tq?tqx=out:csv&sheet={type.Name}";

    public static IEnumerable<Type> GetAllItemTypes() => 
        typeof(Item).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Item))).ToList();
    
    public static void LoadImportedData(DataTable csvFile, Type dataType) => Database.Load(csvFile, dataType);

    private void Load(DataTable csvFile, Type dataType)
    {
        string itemsPath = Path.Combine("res://Resources/Items", dataType.Name);
        GD.Print("Target path " + itemsPath);
        List<Item> items = new();
        foreach (DataRow dataRow in csvFile.Rows)
            items.AddIfNotNull(CreateItem(dataRow, dataType));

        Load(items, _items, itemsPath, x => x.GetType() == dataType);
    }

    private static Item CreateItem(DataRow dataRow, Type type)
    {
        Item itemInstance = (Item)Activator.CreateInstance(type);
        if (itemInstance == null) 
            return null;
        
        itemInstance.Setup(dataRow);
        return itemInstance;
    }
}