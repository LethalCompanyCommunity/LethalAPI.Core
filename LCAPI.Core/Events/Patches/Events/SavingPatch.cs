// -----------------------------------------------------------------------
// <copyright file="SavingPatch.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Events.Patches.Events;

using Attributes;
using Handlers;

/// <summary>
///     Patches the <see cref="Handlers.Server.Saving"/> event.
/// </summary>
// [HarmonyPatch(typeof(), nameof())]
[EventPatch(typeof(Server), nameof(Server.Saving))]
internal sealed class SavingPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
    }
}