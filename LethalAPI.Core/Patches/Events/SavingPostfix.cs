// -----------------------------------------------------------------------
// <copyright file="SavingPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LethalAPI.Core.Patches.Events;

using LethalAPI.Core.Events.Attributes;
using LethalAPI.Core.Events.EventArgs;
using LethalAPI.Core.Events.Handlers;

/// <summary>
///     Patches the <see cref="Server.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveGameValues))]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal static class SaveGameValuesPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
         Server.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.GameValues));
    }
}

/// <summary>
///     Patches the <see cref="Server.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveLocalPlayerValues))]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal static class SaveLocalPlayerValues
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
         Server.Saving.InvokeSafely(new SavingEventArgs("LCGeneralSaveData", SaveItem.LocalPlayerValues));
    }
}

/// <summary>
///     Patches the <see cref="Server.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveItemsInShip))]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal static class SaveItemsInShipPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
         Server.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.ShipItems));
    }
}

/// <summary>
///     Patches the <see cref="Server.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.ConvertUnsellableItemsToCredits))]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal static class SaveUnsellableItemsPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
         Server.Saving.InvokeSafely(new SavingEventArgs(__instance.currentSaveFileName, SaveItem.UnsellableItems));
    }
}