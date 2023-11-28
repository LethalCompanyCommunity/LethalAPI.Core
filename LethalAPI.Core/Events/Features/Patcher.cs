// -----------------------------------------------------------------------
// <copyright file="Patcher.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments.
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Features;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Attributes;
using Interfaces;

/// <summary>
/// A tool for patching.
/// </summary>
public class Patcher
{
    /// <summary>
    /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private static int patchesCounter;

    /// <summary>
    /// Initializes a new instance of the <see cref="Patcher"/> class.
    /// </summary>
    internal Patcher()
    {
        Harmony = new($"lethalapi.events.{++patchesCounter}");
    }

    /// <summary>
    /// Gets a <see cref="HashSet{T}"/> that contains all patch types that haven't been patched.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public static HashSet<Type> UnpatchedTypes { get; private set; } = Events.UseDynamicPatching ? GetNonEventPatchTypes() : GetAllPatchTypes();

    /// <summary>
    /// Gets a <see cref="HashSet{T}"/> that contains all patch types that have been patched.
    /// </summary>
    public static HashSet<Type> PatchedTypes { get; } = new();

    /// <summary>
    /// Gets a set of types and methods for which LethalAPI patches should not be run.
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public static HashSet<MethodBase> DisabledPatchesHashSet { get; } = new();

    /// <summary>
    /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public HarmonyLib.Harmony Harmony { get; }

    /// <summary>
    /// Patches all events that target a specific <see cref="ILethalApiEvent"/>.
    /// </summary>
    /// <param name="event">The <see cref="ILethalApiEvent"/> all matching patches should target.</param>
    public void Patch(ILethalApiEvent @event)
    {
        try
        {
            List<Type> types = new (GetAllPatchTypes().Where(x => x.GetCustomAttributes<EventPatchAttribute>().Any((epa) => epa.Event == @event)));

            Log.Debug($"Patching event for {types.Count} types.", Events.DebugPatches, "LethalAPI-Patcher");
            foreach (Type type in types)
            {
                if (PatchedTypes.Contains(type))
                {
                    Log.Debug($"Type {type.FullName} has already been patched.", Events.DebugPatches, "LethalAPI-Patcher");
                    continue;
                }

                PatchedTypes.Add(type);
                List<MethodInfo> methodInfos = new PatchClassProcessor(Harmony, type).Patch();
                if (DisabledPatchesHashSet.Any(x => methodInfos.Contains(x)))
                    ReloadDisabledPatches();
                UnpatchedTypes.Remove(type);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Patching by event failed!\n{ex}");
        }
    }

    /// <summary>
    /// Patches all events.
    /// </summary>
    /// <param name="failedPatch">the number of failed patch returned.</param>
    /// <param name="totalPatches">the number of total patches attempted.</param>
    public void PatchAll(out int failedPatch, out int totalPatches)
    {
        totalPatches = 0;
        failedPatch = 0;

        try
        {
            List<Type> toPatch = new (UnpatchedTypes);
            foreach (Type patch in toPatch)
            {
                totalPatches++;
                try
                {
                    PatchedTypes.Add(patch);
                    Harmony.CreateClassProcessor(patch).Patch();
                    UnpatchedTypes.Remove(patch);
                    Log.Debug($"Patching type '{patch.FullName}'", Events.DebugPatches, "LethalAPI-Patcher");
                }
                catch (HarmonyException exception)
                {
                    Log.Error($"Patching by attributes failed!\n{exception}");

                    failedPatch++;
                }
            }

            Log.Debug("Events patched by attributes successfully!");
        }
        catch (Exception exception)
        {
            Log.Error($"Patching by attributes failed!\n{exception}");
        }
    }

    /// <summary>
    /// Checks the <see cref="DisabledPatchesHashSet"/> list and un-patches any methods that have been defined there. Once un-patching has been done, they can be patched by plugins, but will not be re-patchable by LethalAPI until a server reboot.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public void ReloadDisabledPatches()
    {
        foreach (MethodBase method in DisabledPatchesHashSet)
        {
            Harmony.Unpatch(method, HarmonyPatchType.All, Harmony.Id);

            Log.Info($"Unpatched {method.Name}");
        }
    }

    /// <summary>
    /// Unpatches all events.
    /// </summary>
    public void UnpatchAll()
    {
        Log.Debug("Un-patching events...");
        HarmonyLib.Harmony.UnpatchID(Harmony.Id);
        UnpatchedTypes = GetAllPatchTypes();
        PatchedTypes.Clear();

        Log.Debug("All events have been unpatched. Goodbye!");
    }

    /// <summary>
    /// Gets all types that have a <see cref="HarmonyPatch"/> attributed to them.
    /// </summary>
    /// <returns>A <see cref="HashSet{T}"/> of all patch types.</returns>
    private static HashSet<Type> GetAllPatchTypes()
    {
        HashSet<Type> types = new ();
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            try
            {
                if (type.GetCustomAttribute<HarmonyPatch>() is null)
                    continue;

                types.Add(type);
            }
            catch (TypeLoadException)
            {
            }
        }

        return types;
    }

    /// <summary>
    /// Gets all types that have a <see cref="HarmonyPatch"/> attributed to them, but don't have an <see cref="EventPatchAttribute"/> attribute.
    /// </summary>
    /// <returns>A <see cref="HashSet{T}"/> of all patch types.</returns>
    private static HashSet<Type> GetNonEventPatchTypes()
    {
        HashSet<Type> types = new ();
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            try
            {
                if (type.GetCustomAttribute<HarmonyPatch>() is null)
                    continue;

                if (type.GetCustomAttributes<EventPatchAttribute>().Any())
                    continue;

                if (type.GetCustomAttributes<EventPatchAttribute>().Any())
                    continue;

                types.Add(type);
            }
            catch (TypeLoadException)
            {
            }
        }

        return types;
    }
}
