// -----------------------------------------------------------------------
// <copyright file="SaveManager.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Enums;
using Interfaces;
using Loader;
using Newtonsoft.Json;

/// <summary>
/// The global save manager.
/// </summary>
public static class SaveManager
{
    /// <summary>
    /// Gets the current save slot that the player has selected.
    /// </summary>
    /// <seealso cref="GameNetworkManager.currentSaveFileName"/>
    public static SaveSlots CurrentSaveSlot => GameNetworkManager.Instance.currentSaveFileName switch
    {
        // Note: This slot will never be selected, it is just here as a quick reference.
        "LCGeneralSaveData" => SaveSlots.GlobalSlot,
        "LCSaveFile1" => SaveSlots.Slot1,
        "LCSaveFile2" => SaveSlots.Slot2,
        "LCSaveFile3" => SaveSlots.Slot3,
        _ => SaveSlots.Slot1,
    };

    /// <summary>
    /// Gets or sets a dictionary of plugin save data.
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Local
    private static Dictionary<string, SaveItemCollection> Saves { get; set; } = new();

    /// <summary>
    /// Gets the directory where saves are stored.
    /// </summary>
    private static string LCSaveDirectory => UnityEngine.Application.persistentDataPath;

    /// <summary>
    /// Gets the name of the file which global modded data is stored.
    /// </summary>
    private static string GlobalModdedSaveFileName => "LCGlobalModdedSaveData";

    /// <summary>
    /// Gets the name of the <see cref="CurrentSaveSlot">currently selected file</see> which the local modded data is stored.
    /// </summary>
    private static string LocalModdedSaveFileName => CurrentSaveSlot switch
    {
        SaveSlots.Slot1 => "LCModdedSaveFile1",
        SaveSlots.Slot2 => "LCModdedSaveFile2",
        SaveSlots.Slot3 => "LCModdedSaveFile3",
        _ => "LCModdedSaveFile1",
    };

    /// <summary>
    /// Loads the plugin-save information from the selected save file for the calling plugin.
    /// </summary>
    public static void LoadFromFile()
    {
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        if (plugin.SaveHandler is null)
        {
            Log.Warn($"Plugin '{plugin.Name}' has no save handler!.");
            return;
        }

        LoadData(plugin);
    }

    /// <summary>
    /// Loads the global plugin-save information from the global save file for the calling plugin.
    /// </summary>
    public static void LoadFromGlobalFile()
    {
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        if (plugin.SaveHandler is null)
        {
            Log.Warn($"Plugin '{plugin.Name}' has no save handler!.");
            return;
        }

        LoadData(plugin, true);
    }

    /// <summary>
    /// Saves the latest plugin-save information to the selected save file for the calling plugin.
    /// </summary>
    public static void SaveToFile()
    {
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        if (plugin.SaveHandler is null)
        {
            Log.Warn($"Plugin '{plugin.Name}' has no save handler!.");
            return;
        }

        SaveData(plugin);
    }

    /// <summary>
    /// Saves the latest global plugin-save information to the global save file for the calling plugin.
    /// </summary>
    public static void SaveToGlobalFile()
    {
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        if (plugin.SaveHandler is null)
        {
            Log.Warn($"Plugin '{plugin.Name}' has no save handler!.");
            return;
        }

        SaveData(plugin, true);
    }

    /// <summary>
    /// Saves a plugin's save-data.
    /// </summary>
    /// <param name="plugin">The plugin to load.</param>
    /// <param name="global">Should the global data instance be saved.</param>
    internal static void SaveData(IPlugin<IConfig> plugin, bool global = false)
    {
    }

    /// <summary>
    /// Loads a plugin's save-data.
    /// </summary>
    /// <param name="plugin">The plugin to load.</param>
    /// <param name="global">Should the global data instance be loaded.</param>
    internal static void LoadData(IPlugin<IConfig> plugin, bool global = false)
    {
    }

    /// <summary>
    /// Gets the path to the <see cref="CurrentSaveSlot">currently selected save slot</see>.
    /// </summary>
    /// <param name="global">Whether to save with the global slot or not.</param>
    /// <returns>The path to the <see cref="CurrentSaveSlot">currently selected save slot</see>.</returns>
    private static string GetCurrentSavePath(bool global = false) => Path.Combine(LCSaveDirectory, global ? GlobalModdedSaveFileName : LocalModdedSaveFileName);

    private static void DeserializeAllSaves(bool global = false)
    {
        string path = GetCurrentSavePath();
        string data = File.ReadAllText(path);
        Dictionary<string, List<SaveItem>>? values = JsonConvert.DeserializeObject<Dictionary<string, List<SaveItem>>>(data);
        if (values is null)
        {
            Log.Error("The save file has been corrupted and could not be loaded.");
            Log.Debug($"Slot: {(global ? LocalModdedSaveFileName : GlobalModdedSaveFileName)}. Null or empty save slot.");
            return;
        }

        foreach (KeyValuePair<string, List<SaveItem>> kvp in values)
        {
            SaveItemCollection collection = new ();
            foreach (SaveItem saveItem in kvp.Value)
            {
                collection.TryAddItem(saveItem, out _);
            }

            collection.MarkAsLoaded();
            Saves.Add(kvp.Key, collection);
        }
    }

    private static IPlugin<IConfig>? GetCallingPlugin(int framesToSkip = 1)
    {
        Type? type = new StackTrace().GetFrame(framesToSkip).GetMethod().DeclaringType;
        if (type is null)
            return null;

        return PluginLoader.GetPlugin(type.Assembly);
    }
}