#if TOOLS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endif

using System.Text.RegularExpressions;
using Godot;

namespace FT.Data;

public partial class ItemDatabaseBase<T, TI> : Resource where T : ItemBase where TI : ItemDatabaseBase<T, TI>
{
    public static string civName = "England";
    [Export] protected Item[] _items;
    
    public static TI Database => _database ??= GD.Load($"Resources/{civName}{typeof(TI).Name}.tres") as TI;
    private static TI _database;

    public static T Get(int id) => Database._items.FirstOrDefault(item => item.Id == id) as T;
    public static TT Get<TT>(int id) where TT : T => Get(id) as TT;
    public static T Get(string name) => Database._items.FirstOrDefault(item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase)) as T;
    public static TT Get<TT>(string name) where TT : Item => Get(name) as TT;
    public static TT[] GetAllOfType<TT>() where TT : T => Database._items.OfType<TT>().ToArray();
    
    
#if TOOLS
    [ExportCategory("Editor Only")]
    [Export] protected string _spreadsheetId;
    [Export] protected string _typesToDownloadFolderLocation;
    [Export] protected string _itemDatabaseLocation;
    [Export] protected string _downloadLocation;

    protected void CheckAndCreateDirectories(string directoryPath)
    {
        string pathWithoutRes = directoryPath[6..];
        string[] folders = pathWithoutRes.Split('/', '\\');
        string globalizedPath = ProjectSettings.GlobalizePath("res://");
        foreach (string folder in folders)
        {
            globalizedPath = Path.Combine(globalizedPath, folder);
            if (Directory.Exists(globalizedPath))
                continue;

            try
            {
                Directory.CreateDirectory(globalizedPath);
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to create directory: {globalizedPath}. Error: {e.Message}");
                return;
            }
        }
    }
    
    protected void Load(List<T> values, string databaseName, string targetsPath, Func<T, bool> filter)
    {
        ItemDatabase itemDatabase = GD.Load<ItemDatabase>($"{_itemDatabaseLocation}{databaseName}{nameof(ItemDatabase)}.tres");
        T[] targets = itemDatabase._items as T[];

        if (!values.Any())
            return;

        Type[] types = values.Select(x => x.GetType()).Distinct().ToArray();
        if (types.Length != 1)
        {
            GD.PrintErr("Assertion failed: types.Count should be 1 but is " + types.Length);
            return;
        }
        
        string CreateItemPath(T item)
        {
            string itemName = $"{item.GetType().Name}_{item.Name.Replace(" ", "")}.tres";
            string uniquePath = Path.Combine(targetsPath, itemName);
            return uniquePath;
        }
        
        string GetItemPath(Item item)
        {
            string itemType = item.GetType().Name;
            string itemName = item.Name.Replace(" ", "");
            return Path.Combine($"res://Resources/Items/{itemType}/{itemType}_{itemName}.tres");    // Extend path later on
        }

       ItemEqualityComparer comparer = new();
       Dictionary<T, string> itemPaths = new();
       foreach (T item in targets.Where(filter))
           itemPaths.TryAdd(item, GetItemPath(item as Item));

        List<(T item, string)> itemsToAdd = values.Except(targets, comparer)
            .Select(item => (item, CreateItemPath(item))).ToList();

        List<(T item, string)> itemsToUpdate = values.Intersect(targets, comparer)
            .Select(item => (item, itemPaths[item])).ToList();
            
        List<(T item, string)> itemsToDelete = targets.Where(filter).Except(values, comparer)
            .Select(item => (item, itemPaths[item])).ToList();

        foreach ((T item, string path) in itemsToUpdate)
        {
           if (!ResourceLoader.Exists(path)) 
               continue;
           
           int index = Array.FindIndex(_items, x => x?.Name == item.Name);
           if (index == -1)
               continue;
           
           ResourceSaver.Save(item, path);
        }

        foreach ((T item, string path) in itemsToDelete)
        {
           int index = Array.FindIndex(_items, i => i?.Name == item.Name);
           if (index == -1) 
               continue;
        
           _items[index] = null;
        
           string globalizedPath = ProjectSettings.GlobalizePath(path);
           if (File.Exists(globalizedPath))
               File.Delete(globalizedPath);
        
           string globalizedPathToTres = ProjectSettings.GlobalizePath(itemDatabase.ResourcePath);
           string content = File.ReadAllText(globalizedPathToTres);
           string pattern = $@"\[\w+_resource type=""Resource"".*path=""{Regex.Escape(path)}"".*\]";
           string newContent = Regex.Replace(content, pattern, "");
        
           File.WriteAllText(globalizedPathToTres, newContent);
        }
       
        List<Item> savedItems = itemsToAdd.Select(itemAndPath =>
            {
                (T item, string path) = itemAndPath;
                return ResourceSaver.Save(item, path) == Error.Ok ? (Item)ResourceLoader.Load(path) : null;
            }).Where(item => item != null).ToList();

        itemDatabase._items = itemDatabase._items.Where(item => item != null).Concat(savedItems).ToArray();
    }
    
    private class ItemEqualityComparer : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }
        
        public int GetHashCode(T obj) => obj.GetHashCode();
    }
    
#endif
}