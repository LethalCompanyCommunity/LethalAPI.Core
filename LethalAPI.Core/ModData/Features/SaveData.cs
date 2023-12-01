// -----------------------------------------------------------------------
// <copyright file="SaveData.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Features;

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using BepInEx.Bootstrap;
using LethalAPI.Core.Patches.Events.Server;
using LethalAPI.Core.ModData;

/// <summary>
/// The class that handle your custom mod save data.
/// </summary>
//todo: get a better name
public class SaveData : IPrefixableItem
{
    /// <summary>
    /// Indicates whether or not the data has been loaded yet.
    /// </summary>
    protected bool loaded;

    /// <summary>
    /// The internal value of the data.
    /// </summary>
    protected object value;

    /// <inheritdoc/>
    public string Prefix { get; init; }

    /// <summary>
    /// Gets the type of the <see cref="value"/>.
    /// </summary>
    public Type Type { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData"/> class.
    /// </summary>
    /// <param name="prefix">The prefix of the item.</param>
    /// <param name="type">The type of this data.</param>
    public SaveData(string prefix, Type type)
    {
        Prefix = prefix;
        Type = type;
        loaded = false;
    }

    /// <summary>
    /// Gets or sets the value of the data.
    /// </summary>
    public object Value
    {
        get
        {
            if (!loaded)
            {
                GetValue();
            }

            return value;
        }

        set
        {
            this.value = value;
            SaveValue();
        }
    }

    /// <summary>
    /// Saves the value to the mod save file.
    /// </summary>
    protected virtual void SaveValue()
    {
        loaded = true;
    }

    /// <summary>
    /// Gets the value to the mod save file.
    /// </summary>
    protected virtual void GetValue()
    {
        loaded = true;
    }
}
