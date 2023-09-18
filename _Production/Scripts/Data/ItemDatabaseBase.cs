using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;

namespace FT.Data;

[GlobalClass]
public partial class ItemDatabaseBase<T, TI> : Resource where T : ItemBase where TI : ItemDatabaseBase<T, TI>
{
    [Export] protected string _spreadsheetId;
    [Export] protected Item[] _items;

    public static TI Database => _database ??= GD.Load("Resources/" + typeof(TI).Name + ".tres") as TI;
    private static TI _database;
    
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

    private List<T> temporaryList = new();
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
            GD.Print(Path.Combine($"res://Resources/Items/{itemType}/{itemType}_{itemName}.tres"));
            return Path.Combine($"res://Resources/Items/{itemType}/{itemType}_{itemName}.tres");
        }
        
        ItemEqualityComparer comparer = new();
        Dictionary<T, string> itemPaths = targets.Where(filter)
            .ToDictionary(item => item, item => GetItemPath(item as Item));
            
        List<(T item, string)> itemsToAdd = values.Except(targets, comparer)
            .Select(item => (item, CreateItemPath(item))).ToList();

        List<(T item, string)> itemsToUpdate = values.Intersect(targets, comparer)
            .Select(item => (item, itemPaths[item])).ToList();
        //    
        //var itemsToDelete = targets.Where(filter).Except(values, comparer)
        //    .Select(item => (item, itemPaths[item])).ToList();
        
        foreach ((T item, string path) in itemsToUpdate)
        {
            Item serializedItem = ResourceLoader.Load(path) as Item;
            if (item == null)
                continue;
            
            for (int i = 0; i < _items.Length; ++i)
            {
                if (_items[i] == null || _items[i].Name != serializedItem.Name) 
                    continue;
                
                GD.Print("Updating " + serializedItem.GetType());
                _items[i] = serializedItem;
                break;
            }
        }
        
        //List<Item> savedItems = itemsToAdd.Select(itemAndPath =>
        //    {
        //        (T item, string path) = itemAndPath;
        //        return ResourceSaver.Save(item, path) == Error.Ok ? (Item)ResourceLoader.Load(path) : null;
        //    }).Where(item => item != null).ToList();
        //_items = _items.Where(item => item != null).Concat(savedItems).ToArray();
    }
}