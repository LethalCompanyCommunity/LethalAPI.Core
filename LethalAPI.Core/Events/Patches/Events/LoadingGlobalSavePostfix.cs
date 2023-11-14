// -----------------------------------------------------------------------
// <copyright file="LoadingGlobalSavePostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Events.Patches.Events;

using Attributes;
using EventArgs;
using Handlers;

#pragma warning disable SA1402

/// <summary>
///     Patches the <see cref="Handlers.Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadingGlobalSavePostfix
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        Server.OnLoadingSave(new LoadingSaveEventArgs("LCGeneralSaveData", LoadedItem.LastSelectedSave));
    }
}

/// <summary>
///     Patches the <see cref="Handlers.Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SpawnUnlockable))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SpawnUnlockablePostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.OnLoadingSave(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SpawnUnlockable));
    }
}

/// <summary>
///     Patches the <see cref="Handlers.Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LoadUnlockables))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadUnlockablesPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.OnLoadingSave(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadUnlockables));
    }
}

/// <summary>
///     Patches the <see cref="Handlers.Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.LoadShipGrabbableItems))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadShipGrabbableItemsPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.OnLoadingSave(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadShipGrabbableItems));
    }
}

/// <summary>
///     Patches the <see cref="Handlers.Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.SetTimeAndPlanetToSavedSettings))]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SetTimeAndPlanetPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        Server.OnLoadingSave(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SetTimeAndPlanetToSavedSettings));
    }
}