// -----------------------------------------------------------------------
// <copyright file="SavingPostfix.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable InconsistentNaming
namespace LCAPI.Core.Events.Patches.Events;

using Attributes;
using EventArgs;
using Handlers;

/// <summary>
///     Patches the <see cref="Handlers.Server.Saving"/> event.
/// </summary>
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.SaveGame))]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal sealed class SavingPostfix
{
    [HarmonyPostfix]
    private static void Postfix(GameNetworkManager __instance)
    {
         Server.OnSaving(new SavingEventArgs(__instance.currentSaveFileName));
    }
}