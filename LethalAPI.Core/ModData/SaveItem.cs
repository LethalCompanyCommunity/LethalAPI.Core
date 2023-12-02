// -----------------------------------------------------------------------
// <copyright file="SaveItem.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1402 // File may only contain a single type.
namespace LethalAPI.Core.ModData;

using Interfaces;

/// <summary>
/// Represents and item that can be saved.
/// </summary>
public class SaveItem : IPrefixableItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveItem"/> class.
    /// </summary>
    /// <param name="prefix">The prefix to store the type as.</param>
    /// <param name="value">The value of the type.</param>
    public SaveItem(string prefix, object value)
    {
        this.Prefix = prefix;
        this.Value = value;
        this.Type = value.GetType();
    }

    /// <inheritdoc />
    public string Prefix { get; }

    /// <summary>
    /// Gets or sets the Value of the item.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Gets the type of the <see cref="Value"/>.
    /// </summary>
    public Type Type { get; }
}

/// <summary>
/// A <see cref="SaveItem"/> but as the type.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
public sealed class SaveItem<T> : SaveItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveItem{T}"/> class.
    /// </summary>
    /// <param name="prefix">The prefix to store the type as.</param>
    /// <param name="value">The value of the type.</param>
    public SaveItem(string prefix, T value)
        : base(prefix, value!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveItem{T}"/> class.
    /// </summary>
    /// <param name="prefix">The prefix to store the type as.</param>
    /// <param name="value">The value of the type.</param>
    public SaveItem(string prefix, object value)
        : base(prefix, value)
    {
    }

    /// <summary>
    /// Gets or sets the value of the item.
    /// </summary>
    public new T Value
    {
        get => (T)base.Value;
        set => base.Value = value!;
    }
}