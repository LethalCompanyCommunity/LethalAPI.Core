// -----------------------------------------------------------------------
// <copyright file="PreInitStartScreenPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Events;

using Core.Events.Attributes;

/// <summary>
///     Patches the <see cref="LethalAPI.Core.Events.Handlers.Server.GameOpened"/> event.
/// </summary>
[EventPatch(typeof(LethalAPI.Core.Events.Handlers.Server), nameof(LethalAPI.Core.Events.Handlers.Server.GameOpened))]
[HarmonyPatch(typeof(PreInitSceneScript), nameof(PreInitSceneScript.Start))]
internal static class PreInitStartScreenPostfix
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        LethalAPI.Core.Events.Handlers.Server.GameOpened.InvokeSafely();
    }
}