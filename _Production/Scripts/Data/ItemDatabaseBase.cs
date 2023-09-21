#if TOOLS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
#endif 

using Godot;

namespace FT.Data;

public partial class ItemDatabaseBase<T, TI> : Resource where T : ItemBase where TI : ItemDatabaseBase<T, TI>
{
    [Export] protected Item[] _items;

    public static TI Database => _database ??= GD.Load("Resources/" + typeof(TI).Name + ".tres") as TI;
    private static TI _database;

    private static T Get(int id) => Database._items.FirstOrDefault(item => item.Id == id) as T;
    public static TT Get<TT>(int id) where TT : T => Get(id) as TT;
    public static TT[] GetAllOfType<TT>() where TT : T => Database._items.OfType<TT>().ToArray();
    
    
#if TOOLS
    [ExportCategory("Editor Only")]
    [Export] protected string _spreadsheetId;
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

    protected void Load(List<T> values, T[] targets, string targetsPath, Func<T, bool> filter)
    {
        if (!values.Any())
            return;

        List<Type> types = values.Select(x => x.GetType()).Distinct().ToList();
        if (types.Count != 1)
        {
            GD.PrintErr("Assertion failed: types.Count should be 1 but is " + types.Count);
            return;
        }
        
        string CreateItemPath(T item)
        {
            string itemName = $"{item.GetType().Name}_{item.Name.Replace(" ", "")}.tres";

            // Generate a unique path for the resource
            int counter = 0;
            string uniquePath = Path.Combine(targetsPath, itemName);
    
            while (File.Exists(uniquePath))
            {
                counter++;
                uniquePath = Path.Combine(targetsPath, $"{item.GetType().Name}_{item.Name.Replace(" ", "")}_{counter}.tres");
            }
    
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
        
        const string pathToTres = "res://Resources/ItemDatabase.tres";  // Ugh, terrible
        foreach ((T item, string path) in itemsToDelete)
        {
           int index = Array.FindIndex(_items, i => i?.Name == item.Name);
           if (index == -1) 
               continue;

           _items[index] = null;

           string globalizedPath = ProjectSettings.GlobalizePath(path);
           if (File.Exists(globalizedPath))
               File.Delete(globalizedPath);

           string globalizedPathToTres = ProjectSettings.GlobalizePath(pathToTres);
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
        _items = _items.Where(item => item != null).Concat(savedItems).ToArray();
    }
    
#endif
}