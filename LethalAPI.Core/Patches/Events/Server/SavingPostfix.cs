// -----------------------------------------------------------------------
// <copyright file="SavingPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events.Server;

using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs.Server;

/// <summary>
///     Patches the <see cref="HandlersServer.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), "SaveGameValues")]
[EventPatch(typeof(HandlersServer), nameof(HandlersServer.Saving))]
internal static class SaveGameValuesPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
        HandlersServer.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.GameValues));
    }
}

/// <summary>
///     Patches the <see cref="HandlersServer.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), "SaveLocalPlayerValues")]
[EventPatch(typeof(HandlersServer), nameof(HandlersServer.Saving))]
internal static class SaveLocalPlayerValues
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
        HandlersServer.Saving.InvokeSafely(new SavingEventArgs("LCGeneralSaveData", SaveItem.LocalPlayerValues));
    }
}

/// <summary>
///     Patches the <see cref="HandlersServer.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), "SaveItemsInShip")]
[EventPatch(typeof(HandlersServer), nameof(HandlersServer.Saving))]
internal static class SaveItemsInShipPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
        HandlersServer.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.ShipItems));
    }
}

/// <summary>
///     Patches the <see cref="HandlersServer.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), "ConvertUnsellableItemsToCredits")]
[EventPatch(typeof(HandlersServer), nameof(HandlersServer.Saving))]
internal static class SaveUnsellableItemsPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
        HandlersServer.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.UnsellableItems));
    }
}