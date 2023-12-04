// -----------------------------------------------------------------------
// <copyright file="LobbyDataIsJoinableTranspiler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Features;

using System.Collections.Generic;
using System.Reflection.Emit;

using Models;
using Steamworks.Data;

/// <summary>
///     Patches <see cref="GameNetworkManager.LobbyDataIsJoinable" />.
///     Overrides the joinable to use __joinable instead. This is necessary in case there are required plugins in the
///     lobby, since the API will then disable joining for vanilla clients.
/// </summary>
/// <seealso cref="GameNetworkManager.LobbyDataIsJoinable" />
// ReSharper disable UnusedMember.Local
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.LobbyDataIsJoinable))]
[HarmonyPriority(Priority.Last)]
[HarmonyWrapSafe]
internal static class LobbyDataIsJoinableTranspiler
{
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new CodeMatcher(instructions)
            .SearchForward(instruction => instruction.OperandIs(LobbyMetadata.Joinable))
            .ThrowIfInvalid("Could not find joinable")
            .RemoveInstructions(4)
            .Insert(new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(LobbyDataIsJoinableTranspiler), nameof(IsJoinable))))
            .InstructionEnumeration();
    }

    private static bool IsJoinable(ref Lobby lobby)
    {
        return lobby.GetData(LobbyMetadata.JoinableModded) == "true" || lobby.GetData(LobbyMetadata.Joinable) == "true";
    }
}