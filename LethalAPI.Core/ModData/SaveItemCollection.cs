// -----------------------------------------------------------------------
// <copyright file="SaveItemCollection.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System;
using System.Collections.Generic;
using System.Reflection;

using Features;
using Interfaces;

/// <summary>
/// Stores a collection of <see cref="SaveItem">SaveItem's</see>.
/// </summary>
public sealed class SaveItemCollection : Collection<SaveItem>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveItemCollection"/> class.
    /// </summary>
    public SaveItemCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveItemCollection"/> class.
    /// </summary>
    /// <param name="items">The items to use in the collection.</param>
    public SaveItemCollection(List<SaveItem> items)
        : base(items)
    {
    }

    /// <summary>
    /// Gets the collection as a list of saveItems.
    /// </summary>
    internal List<SaveItem> AsList => this.Items;

    /// <summary>
    /// Gets a new save instance from a type, with the values present here.
    /// </summary>
    /// <param name="type">The type to make the save from.</param>
    /// <returns>The newly generated save. Null if an error occured.</returns>
    internal ISave? GetNewSaveFromValues(Type type)
    {
        try
        {
            object obj = Activator.CreateInstance(type);
            UpdateObjectWithValues(ref obj);
            return obj as ISave;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Updates the collection with the values from an object.
    /// </summary>
    /// <param name="obj">The object to read the values from.</param>
    internal void UpdateCollectionWithObjectValues(object obj)
    {
        foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (property.GetMethod is null)
                continue;

            object value = property.GetValue(obj);
            if (this.itemsByPrefix.ContainsKey(property.Name))
            {
                SaveItem og = this.itemsByPrefix[property.Name];
                if (og.Type != value.GetType())
                {
                    Log.Debug($"Overwriting mod data for item {property.Name}");
                    og = new SaveItem(og.Prefix, value);
                }
                else
                {
                    og.Value = value;
                }

                this.itemsByPrefix[property.Name] = og;
                int index = this.Items.FindIndex(x => x.Prefix == property.Name);
                this.Items[index] = og;
                continue;
            }

            SaveItem item = new(property.Name, value);
            this.Items.Add(item);
            this.itemsByPrefix.Add(property.Name, item);
        }
    }

    /// <summary>
    /// Updates an object with the values from this collection.
    /// </summary>
    /// <param name="obj">The object to overwrite the values with.</param>
    private void UpdateObjectWithValues(ref object obj)
    {
        foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (property.SetMethod is null)
                continue;

            if (!this.TryGetItem(property.Name, out SaveItem? item))
                continue;

            if(property.PropertyType != item.Type)
                continue;

            FieldInfo? backingField = obj.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (backingField is null)
            {
                Log.Warn($"Could not find backing field for property {property.Name}. This may break things!");
                property.SetValue(obj, item.Value);
                continue;
            }

            backingField.SetValue(obj, item.Value);
        }
    }
}