// -----------------------------------------------------------------------
// <copyright file="SaveManager.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Enums;
using Interfaces;
using Internal;
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
    /// Gets or sets a dictionary of plugin local save data.
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Local
    private static Dictionary<string, SaveItemCollection> LocalSaves { get; set; } = new();

    /// <summary>
    /// Gets or sets a dictionary of plugin global save data.
    /// </summary>
    // ReSharper disable once CollectionNeverQueried.Local
    private static Dictionary<string, SaveItemCollection> GlobalSaves { get; set; } = new();

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
        Log.Debug("Loading from file.");
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        LoadData(plugin);
    }

    /// <summary>
    /// Loads the global plugin-save information from the global save file for the calling plugin.
    /// </summary>
    public static void LoadFromGlobalFile()
    {
        Log.Debug("Loading from global file.");
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        LoadData(plugin, true);
    }

    /// <summary>
    /// Saves the latest plugin-save information to the selected save file for the calling plugin.
    /// </summary>
    public static void SaveToFile()
    {
        Log.Debug("Saving to file.");
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
            return;
        }

        SaveData(plugin);
    }

    /// <summary>
    /// Saves the latest global plugin-save information to the global save file for the calling plugin.
    /// </summary>
    public static void SaveToGlobalFile()
    {
        Log.Debug("Saving to global file.");
        IPlugin<IConfig>? plugin = GetCallingPlugin();
        if (plugin == null)
        {
            Log.Warn("Only LethalAPI Plugins can load and save data.");
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
        Log.Debug($"Saving data for plugin {plugin.Name}{(global ? " [&2Global&r]" : string.Empty)}.");
        if (global)
        {
            if (plugin.GlobalSaveHandler is IInstanceSave globalSaveHandler)
                plugin.GlobalSaveHandler.DataCollection.UpdateCollectionWithObjectValues(globalSaveHandler.Save);

            GlobalSaves[plugin.Name] = plugin.GlobalSaveHandler.DataCollection;
            SerializeAllSaves(true);
            return;
        }

        if(plugin.LocalSaveHandler is IInstanceSave localSaveHandler)
            plugin.LocalSaveHandler.DataCollection.UpdateCollectionWithObjectValues(localSaveHandler.Save);

        LocalSaves[plugin.Name] = plugin.LocalSaveHandler.DataCollection;
        SerializeAllSaves();
    }

    /// <summary>
    /// Loads a plugin's save-data.
    /// </summary>
    /// <param name="plugin">The plugin to load.</param>
    /// <param name="global">Should the global data instance be loaded.</param>
    internal static void LoadData(IPlugin<IConfig> plugin, bool global = false)
    {
        Log.Debug($"Loading data for plugin {plugin.Name}{(global ? " [&2Global&r]" : string.Empty)}.");
        if (global)
        {
            DeserializeAllSaves(true);
            if (!GlobalSaves.ContainsKey(plugin.Name))
            {
                GenerateNewSaveCollection(plugin, true);
                SerializeAllSaves();
            }

            plugin.GlobalSaveHandler.DataCollection = GlobalSaves.TryGetValue(plugin.Name, out SaveItemCollection? globalSave) ? globalSave : new();
            return;
        }

        DeserializeAllSaves();
        if (!LocalSaves.ContainsKey(plugin.Name))
        {
            GenerateNewSaveCollection(plugin);
            SerializeAllSaves();
        }

        plugin.GlobalSaveHandler.DataCollection = LocalSaves.TryGetValue(plugin.Name, out SaveItemCollection? localSave) ? localSave : new();
    }

    /// <summary>
    /// Gets the path to the <see cref="CurrentSaveSlot">currently selected save slot</see>.
    /// </summary>
    /// <param name="global">Whether to save with the global slot or not.</param>
    /// <returns>The path to the <see cref="CurrentSaveSlot">currently selected save slot</see>.</returns>
    private static string GetCurrentSavePath(bool global = false) => Path.Combine(LCSaveDirectory, global ? GlobalModdedSaveFileName : LocalModdedSaveFileName);

    private static void DeserializeAllSaves(bool global = false)
    {
        Log.Debug($"Loading all plugin data{(global ? " [&2Global&r]" : string.Empty)}.");
        string path = GetCurrentSavePath(global);
        string data = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        Dictionary<string, List<SaveItem>>? values = JsonConvert.DeserializeObject<Dictionary<string, List<SaveItem>>>(data);
        if (values is null)
        {
            if(global)
                GlobalSaves = new();
            else
                LocalSaves = new();

            Log.Error("The save file has been corrupted and could not be loaded.");
            Log.Debug($"Slot: {(global ? LocalModdedSaveFileName : GlobalModdedSaveFileName)}. Null or empty save slot.");

            try
            {
                if(File.Exists(path))
                    File.Delete(path);

                foreach (IPlugin<IConfig> plugin in PluginLoader.Plugins.Values)
                    GenerateNewSaveCollection(plugin, global);

                SerializeAllSaves();
            }
            catch (Exception e)
            {
                Log.Warn("Could not delete the save file.");
                if(CorePlugin.Instance.Config.Debug)
                    Log.Exception(e);
            }

            return;
        }

        foreach (KeyValuePair<string, List<SaveItem>> kvp in values)
        {
            SaveItemCollection collection = new(kvp.Value);
            collection.MarkAsLoaded();
            if((global ? GlobalSaves : LocalSaves).ContainsKey(kvp.Key))
                (global ? GlobalSaves : LocalSaves)[kvp.Key] = collection;
            else
                (global ? GlobalSaves : LocalSaves).Add(kvp.Key, collection);
        }
    }

    private static void GenerateNewSaveCollection(IPlugin<IConfig> plugin, bool global = false)
    {
        Log.Debug($"Generating new plugin data for plugin {plugin.Name}{(global ? " [&2Global&r]" : string.Empty)}.");
        SaveItemCollection collection = new();
        if (global)
        {
            if (plugin.GlobalSaveHandler is IInstanceSave globalSave)
            {
                object? value = null;
                try
                {
                    value = Activator.CreateInstance(globalSave.Save.GetType(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance);
                }
                catch (Exception e)
                {
                    Log.Error($"Could not create an instance of the global save data for type '{globalSave.Save.GetType().FullName}'. This is probably due to an invalid type.");
                    if(CorePlugin.Instance.Config.Debug)
                        Log.Exception(e);
                }

                if(value is not null)
                    collection.UpdateCollectionWithObjectValues(value);
            }

            GlobalSaves.Add(plugin.Name, collection);
            return;
        }

        if (plugin.LocalSaveHandler is IInstanceSave localSave)
        {
            object? value = null;
            try
            {
                value = Activator.CreateInstance(localSave.Save.GetType(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.CreateInstance);
            }
            catch (Exception e)
            {
                Log.Error($"Could not create an instance of the global save data for type '{localSave.Save.GetType().FullName}'. This is probably due to an invalid type.");
                if(CorePlugin.Instance.Config.Debug)
                    Log.Exception(e);
            }

            if(value is not null)
                collection.UpdateCollectionWithObjectValues(value);
        }

        LocalSaves.Add(plugin.Name, collection);
    }

    private static void SerializeAllSaves(bool global = false)
    {
        Log.Debug($"Saving all plugin data{(global ? " [&2Global&r]" : string.Empty)}.");
        Dictionary<string, List<SaveItem>> items = new();
        foreach (KeyValuePair<string, SaveItemCollection> saves in global ? GlobalSaves : LocalSaves)
        {
            items.Add(saves.Key, saves.Value.AsList);
        }

        string json = JsonConvert.SerializeObject(items);
        string path = GetCurrentSavePath(global);
        try
        {
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Log.Error($"Could not save to file '{path}'.");
            if(CorePlugin.Instance.Config.Debug)
                Log.Exception(e);
        }
    }

    private static IPlugin<IConfig>? GetCallingPlugin(int framesToSkip = 1)
    {
        Type? type = new StackTrace().GetFrame(framesToSkip + 1).GetMethod().DeclaringType;
        if (type is null)
            return null;

        return PluginLoader.GetPlugin(type.Assembly);
    }
}