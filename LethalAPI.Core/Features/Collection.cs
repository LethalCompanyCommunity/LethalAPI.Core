// -----------------------------------------------------------------------
// <copyright file="Collection.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Inspiration taken from O5Zereths BananaPlugin / Feature Collection.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Features;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Interfaces;

#pragma warning disable SA1401

/// <summary>
/// Used to contain all a set of items.
/// </summary>
/// <typeparam name="T">The type item the collection holds.</typeparam>
// ReSharper disable MemberCanBePrivate.Global
public class Collection<T> : IEnumerable<T>
    where T : IPrefixableItem
{
    /// <summary>
    /// The items but sorted by a prefix.
    /// </summary>
    protected readonly Dictionary<string, T> itemsByPrefix;

    /// <summary>
    /// The items buy not sort.
    /// </summary>
    protected readonly List<T> Items;

    /// <summary>
    /// Used to notate whether the collection has been loaded or not.
    /// </summary>
    protected bool isLoaded;

    /// <summary>
    /// Initializes a new instance of the <see cref="Collection{T}"/> class.
    /// </summary>
    public Collection()
    {
        this.itemsByPrefix = new ();
        this.Items = new ();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Collection{T}"/> class.
    /// </summary>
    /// <param name="items">The items to use.</param>
    public Collection(List<T> items)
    {
        this.itemsByPrefix = new();
        this.Items = items;
        foreach (T item in this.Items)
        {
            itemsByPrefix.Add(item.Prefix, item);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the collection is loaded.
    /// </summary>
    public bool IsLoaded => this.isLoaded;

    /// <inheritdoc cref="TryGetItem"/>
    public T this[string prefix]
    {
        get
        {
            if (!this.TryGetItem(prefix, out T? result))
            {
                throw new ArgumentOutOfRangeException($"Feature {prefix} does not exist, and cannot be retrieved.");
            }

            if (result is null)
            {
                throw new ArgumentOutOfRangeException($"Feature {prefix} does not exist, and cannot be retrieved.");
            }

            return result;
        }
    }

    /// <summary>
    /// Gets the count of the amount of items in the collection.
    /// </summary>
    /// <returns>The count of items in the collection.</returns>
    public int GetCount() => this.Items.Count;

    /// <summary>
    /// Attempts to get an item by its prefix.
    /// </summary>
    /// <param name="prefix">The prefix to find.</param>
    /// <param name="item">The item, if found.</param>
    /// <returns>A value indicating whether the operation was a success.</returns>
    public bool TryGetItem(string prefix, [NotNullWhen(true)] out T? item)
    {
        return this.itemsByPrefix.TryGetValue(prefix, out item);
    }

    /// <summary>
    /// Gets the enumerator.
    /// </summary>
    /// <returns>An enumerator over the list of items.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return this.Items.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    /// <summary>
    /// Adds an item to the list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="response">The error response.</param>
    /// <returns>A value indicating whether the operation was a success.</returns>
    internal virtual bool TryAddItem(T item, [NotNullWhen(false)] out string? response)
    {
        if (this.isLoaded)
        {
            response = $"Item '{item.Prefix}' could not be added. The collection is already loaded.";
            return false;
        }

        try
        {
            if (this.itemsByPrefix.ContainsKey(item.Prefix))
            {
                response = $"Item '{item.Prefix}' could not be added due to a duplicate prefix.";
                return false;
            }

            this.itemsByPrefix.Add(item.Prefix, item);
            this.Items.Add(item);
            response = null;
            return true;
        }
        catch (Exception e)
        {
            response = $"Error adding feature to list: {e}";
            return false;
        }
    }

    /// <summary>
    /// Marks the collection as loaded, and no more items can be added.
    /// </summary>
    internal void MarkAsLoaded()
    {
        this.isLoaded = true;
    }
}
