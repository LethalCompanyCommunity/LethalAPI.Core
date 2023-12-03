// -----------------------------------------------------------------------
// <copyright file="SaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;

using Interfaces;

/// <summary>
/// Contains abstractions for handling save properties.
/// </summary>
public abstract class SaveHandler
{
    /// <summary>
    /// Gets the primary data collection.
    /// </summary>
    public SaveItemCollection DataCollection { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether or not this save handler is handling global data or local data.
    /// </summary>
    protected bool IsGlobalSave { get; set; }

    /// <summary>
    /// Gets the settings related to the save data, as indicated by the plugin author.
    /// </summary>
    protected SaveDataSettings Settings { get; init; } = null!;

    /// <summary>
    /// Gets the plugin instance this handler represents.
    /// </summary>
    protected IPlugin<IConfig> Plugin { get; init; } = null!;

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
    /// <param name="searchGlobal">Indicates whether or not to search for a global save or a local save instance.</param>
    /// <returns>The save handler that was found.</returns>
    internal static SaveHandler GetSaveHandler(IPlugin<IConfig> plugin, bool searchGlobal = false)
    {
        try
        {
            return new InheritedSaveHandler(plugin, searchGlobal);
        }
        catch (Exception)
        {
            // unused.
        }

        try
        {
            return new PropertySaveHandler(plugin, searchGlobal);
        }
        catch (Exception)
        {
            // unused.
        }

        return new GenericSaveHandler(searchGlobal);
    }
}