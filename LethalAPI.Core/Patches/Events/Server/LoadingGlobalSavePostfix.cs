// -----------------------------------------------------------------------
// <copyright file="LoadingGlobalSavePostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Events.Server;

using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs.Server;
using LethalAPI.Core.Events.Handlers;

#pragma warning disable SA1402

/// <summary>
///     Patches the <see cref="HandlersServer.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), "Start")]
[EventPatch(typeof(HandlersServer), nameof(HandlersServer.LoadingSave))]
internal static class LoadingGlobalSavePostfix
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        HandlersServer.LoadingSave.InvokeSafely(new LoadingSaveEventArgs("LCGeneralSaveData", LoadedItem.LastSelectedSave));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), "SpawnUnlockable")]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SpawnUnlockablePostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        HandlersServer.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SpawnUnlockable));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), "LoadUnlockables")]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadUnlockablesPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        HandlersServer.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadUnlockables));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), "LoadShipGrabbableItems")]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class LoadShipGrabbableItemsPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        HandlersServer.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.LoadShipGrabbableItems));
    }
}

/// <summary>
///     Patches the <see cref="Server.LoadingSave"/> event.
/// </summary>
[HarmonyPatch(typeof(StartOfRound), "SetTimeAndPlanetToSavedSettings")]
[EventPatch(typeof(Server), nameof(Server.LoadingSave))]
internal static class SetTimeAndPlanetPostfix
{
    [HarmonyPostfix]
    private static void Postfix(StartOfRound __instance)
    {
        HandlersServer.LoadingSave.InvokeSafely(new LoadingSaveEventArgs(GameNetworkManager.Instance.currentSaveFileName, LoadedItem.SetTimeAndPlanetToSavedSettings));
    }
}