// -----------------------------------------------------------------------
// <copyright file="SaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;

using Attributes;
using Interfaces;

/// <summary>
/// Contains abstractions for handling save properties.
/// </summary>
public abstract class SaveHandler
{
    /// <summary>
    /// Gets or sets the save instance to use.
    /// </summary>
    public abstract ISave Save { get; set; }

    /// <summary>
    /// Gets the plugin instance this handler represents.
    /// </summary>
    public IPlugin<IConfig> Plugin { get; init; } = null!;

    /// <summary>
    /// Gets the settings related to the save data, as indicated by the plugin author.
    /// </summary>
    public SaveDataAttribute Settings { get; init; } = null!;

    /// <summary>
    /// Loads the save from the saved file location.
    /// </summary>
    /// <param name="global">Should global data be loaded instead of the local data.</param>
    public void LoadData(bool global = false) => SaveManager.LoadData(Plugin, global);

    /// <summary>
    /// Saves the data to the proper save file location.
    /// </summary>
    /// <param name="global">Should global data be saved instead of the local data.</param>
    public void SaveData(bool global = false) => SaveManager.SaveData(Plugin, global);

    /// <summary>
    /// Gets the save handler for a plugin.
    /// </summary>
    /// <remarks>Not all plugins have save handlers! Always check to ensure that a plugin has a save handler.</remarks>
    /// <param name="plugin">The plugin to create the save handler from.</param>
    /// <returns>The save handler if it was found, null if it could not be found.</returns>
    internal static SaveHandler? GetSaveHandler(IPlugin<IConfig> plugin)
    {
        try
        {
            return new InheritedSaveHandler(plugin);
        }
        catch (Exception)
        {
            // Ignore.
        }

        try
        {
            return new PropertySaveHandler(plugin);
        }
        catch (Exception)
        {
            // Ignore
        }

        return null;
    }
}