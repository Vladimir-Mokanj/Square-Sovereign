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
    
    private class ItemEqualityComparer<T> : IEqualityComparer<T> where T : ItemBase
    {
        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null) || x.GetType() != y.GetType()) return false;
            return x.Id == y.Id;
        }
        
        public int GetHashCode(T obj) => obj.GetHashCode();
    }
    
    protected void Load<T>(List<T> values, T[] targets, string targetsPath, Func<T, bool> filter) where T : ItemBase
    {
        if (!values.Any())
            return;
        
        List<Type> types = values.Select(x => x.GetType()).Distinct().ToList();
        if (types.Count != 1)
        {
            GD.PrintErr("Assertion failed: types.Count should be 1 but is " + types.Count);
            return;
        }

        Type firstType = types.First();
        
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
            return Path.Combine("res://Resources/Items/", itemType, $"{itemName}.tres");
        }
        
        var comparer = new ItemEqualityComparer<T>();
        //Dictionary<T, string> itemPaths = targets.Where(filter)
        //    .ToDictionary(item => item, item => GetItemPath(item as Item));
            
        List<(T item, string)> itemsToAdd = values.Except(targets, comparer)
            .Select(item => (item, CreateItemPath(item))).ToList();

        //var itemsToUpdate = values.Intersect(targets, comparer)
        //    .Select(item => (item, itemPaths[item])).ToList();
        //    
        //var itemsToDelete = targets.Where(filter).Except(values, comparer)
        //    .Select(item => (item, itemPaths[item])).ToList();
        
        foreach ((T item, string path) in itemsToAdd)
        {
            GD.Print("Creating " + path);

            if (item is Resource resource)
            {
                // Save the resource
                if (ResourceSaver.Save(resource, path) == Error.Ok)
                {
                    GD.Print("Successfully saved at " + path);

                    // Load the resource
                    if (ResourceLoader.Load(path) is T loadedResource)
                    {
                        targets[0] = loadedResource;
                        GD.Print("Successfully loaded from " + path);
                    }
                    else
                    {
                        GD.PrintErr("Failed to load asset at " + path);
                    }
                }
                else
                {
                    GD.PrintErr("Failed to save asset at " + path);
                }
            }
            else
            {
                GD.PrintErr("Item is not a Resource: " + path);
            }
        }
        
        
        //foreach ((T item, string path) in itemsToAdd)
        //{
        //    GD.Print("Creating " + path);
        //    if (item is Resource resource)
        //    {
        //        ResourceSaver.Save(resource, path);
        //        continue;
        //    }
        //    
        //    GD.PrintErr("Failed to load asset at " + path);
        //}
        //
        //foreach ((T _, string path) in itemsToAdd)
        //{
        //    if (ResourceLoader.Load(path) is T loadedResource)
        //    {
        //        targets.Add(loadedResource);
        //        continue;
        //    }
        //    
        //    GD.PrintErr("Failed to load asset at " + path);
        //}
    }
}