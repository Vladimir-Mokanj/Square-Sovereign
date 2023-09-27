#if TOOLS
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using FT.Tools;
#endif

using Godot;

namespace FT.Data;

[GlobalClass, Tool]
public partial class ItemDatabase : ItemDatabaseBase<Item, ItemDatabase>
{
#if TOOLS
    public string GetDownloadUrl(Type type) => 
        $"https://docs.google.com/spreadsheets/d/{_spreadsheetId}/gviz/tq?tqx=out:csv&sheet={type.Name}";

    public Type[] GetAllItemTypes() => 
        typeof(Item).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Item)) && type.FullName.Contains(_typesToDownloadFolderLocation)).ToArray();
    
    public void LoadImportedData(DataTable csvFile, Type dataType, string databaseResourceName) => Database.Load(csvFile, dataType, databaseResourceName);

    public void LoadGeneralData(string databaseResourceName)
    {
        ItemDatabase itemDatabase = GD.Load<ItemDatabase>($"{_itemDatabaseLocation}{databaseResourceName}{nameof(ItemDatabase)}.tres");
        ItemDatabase generalItemDatabase = GD.Load<ItemDatabase>($"{_itemDatabaseLocation}General{nameof(ItemDatabase)}.tres");
        List<Item> allItems = itemDatabase._items.ToList();
        allItems.AddRange(generalItemDatabase._items);

        itemDatabase._items = allItems.ToArray();
    }

    private void Load(DataTable csvFile, Type dataType, string databaseResourceName)
    {
        string itemsPath = Path.Combine($"{_downloadLocation}{databaseResourceName}", dataType.Name);
        CheckAndCreateDirectories(itemsPath);
        GD.Print("Target path " + itemsPath);
        List<Item> items = new();
        foreach (DataRow dataRow in csvFile.Rows)
            items.AddIfNotNull(CreateItem(dataRow, dataType));
        
        Load(items, databaseResourceName, itemsPath, x => x.GetType() == dataType);
    }

    private static Item CreateItem(DataRow dataRow, Type type)
    {
        Item itemInstance = (Item)Activator.CreateInstance(type);
        if (itemInstance == null) 
            return null;
        
        itemInstance.Setup(dataRow);
        return itemInstance;
    }
    
#endif
}