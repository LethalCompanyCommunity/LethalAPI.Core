// -----------------------------------------------------------------------
// <copyright file="PropertySaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

using Attributes;
using Interfaces;

/// <summary>
/// Contains the implementation for saving to properties.
/// </summary>
internal class PropertySaveHandler : GenericSaveHandler, IInstanceSave
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to search.</param>
    /// <param name="searchGlobal">Indicates whether or not to search for global saves.</param>
    /// <exception cref="MissingMemberException">Thrown if no valid save data property is found in the plugin.</exception>
    internal PropertySaveHandler(IPlugin<IConfig> plugin, bool searchGlobal = false)
        : base(searchGlobal)
    {
        this.IsGlobalSave = searchGlobal;
        this.Plugin = plugin;
        this.PluginInstance = plugin.RootInstance;
        string propertyName = (searchGlobal ? "Global" : "Local") + "SaveData";
        foreach (PropertyInfo propertyInfo in this.PluginInstance.GetType().GetProperties())
        {
            if (propertyInfo.Name == propertyName)
            {
                this.Settings = new SaveDataSettings(true, true);
                this.Property = propertyInfo;
                goto ensureValid;
            }

            if (searchGlobal)
            {
                if (propertyInfo.GetCustomAttribute<GlobalSaveDataAttribute>() is not { } globalSettings)
                    continue;

                this.Settings = globalSettings.Settings;
                goto ensureValid;
            }

            if (propertyInfo.GetCustomAttribute<LocalSaveDataAttribute>() is not { } localSettings)
                continue;

            this.Settings = localSettings.Settings;

            // Check to ensure the property has a getter and setter. If it doesnt, continue.
            ensureValid:
            if (propertyInfo.GetMethod is null || propertyInfo.SetMethod is null)
                continue;

            this.Property = propertyInfo;
            if(this.Settings.AutoSave)
                HookAutoSave();
            Log.Debug($"Successfully created property save handler for plugin {plugin.Name} [{Settings.AutoSave}, {Settings.AutoLoad}].");
            return;
        }

        throw new MissingMemberException($"The {(searchGlobal ? "Global" : "Local")}SaveData property does not exist for this plugin!");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySaveHandler"/> class.
    /// </summary>
    /// <param name="searchGlobal">Indicates whether or not to search for global saves.</param>
    protected PropertySaveHandler(bool searchGlobal)
        : base(searchGlobal)
    {
    }

    /// <inheritdoc />
    public ISave Save
    {
        get => (ISave)this.Property.GetValue(this.PluginInstance);
        set => this.Property.SetValue(this.PluginInstance, value);
    }

    /// <summary>
    /// Gets the plugin instance. This may not be a <see cref="IPlugin{TConfig}"/>.
    /// </summary>
    protected object PluginInstance { get; init; } = null!;

    /// <summary>
    /// Gets the property instance.
    /// </summary>
    protected PropertyInfo Property { get; init; } = null!;

    /// <inheritdoc/>
    public void OnSaveUpdated()
    {
        if (this.Settings.AutoSave)
        {
            this.SaveData();
        }
    }

    /// <summary>
    /// Hooks the auto-saving functions for a save instance.
    /// </summary>
    protected void HookAutoSave()
    {
        Log.Debug("Hooking autosave functions.");
        HarmonyMethod method = new(typeof(PropertySaveHandler), nameof(HandleUpdateTranspiler));

        // CorePlugin.Harmony.Patch(this.Property.SetMethod, method);
        foreach (PropertyInfo property in this.Property.PropertyType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (property.SetMethod is null || property.GetMethod is null)
                continue;

            try
            {
                CorePlugin.Harmony.Patch(property.SetMethod, transpiler: method);
            }
            catch (Exception)
            {
                // unused.
            }
        }
    }

    private static void HandleUpdate()
    {
        // Skip Reflection calls that were apart of our assembly.
        // Call a save otherwise.
        StackTrace trace = new();
        StackFrame[]? frames = trace.GetFrames();
        if (frames is null)
        {
            Log.Debug("Update Handler: Trace was empty.");
            return;
        }

        StringBuilder builder = new();
        builder.AppendLine($"Update Handler: \n Setting Trace Location:");
        for (int i = 0; i < frames.Length; i++)
        {
            StackFrame frame = frames[i];
            MethodBase method = frame.GetMethod();
            builder.AppendLine($"  - {method.DeclaringType?.Name}.{method.Name}()");
        }

        Log.Debug(builder.ToString());
    }

    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> HandleUpdateTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = instructions as List<CodeInstruction> ?? instructions.ToList();
        List<int> injectionIndexes = new();
        for (int i = 0; i < newInstructions.Count; i++)
        {
            if(newInstructions[i].opcode == OpCodes.Ret)
                injectionIndexes.Add(i);
        }

        foreach (int i in injectionIndexes)
        {
            newInstructions.InsertRange(i, new CodeInstruction[]
            {
                new(OpCodes.Callvirt, AccessTools.Method(typeof(PropertySaveHandler), nameof(HandleUpdate))),
            });
        }

        for (int i = 0; i < newInstructions.Count; i++)
            yield return newInstructions[i];
    }
}
