// -----------------------------------------------------------------------
// <copyright file="LobbyDataIsJoinablePostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Features;

using Core.Features;
using Models;
using Steamworks;
using Steamworks.Data;

/// <summary>
///     Patches <see cref="GameNetworkManager.LobbyDataIsJoinable" />.
///     Checks if required plugins are present in the lobby metadata and are the same version as the client.
/// </summary>
/// <seealso cref="GameNetworkManager.LobbyDataIsJoinable" />
[HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.LobbyDataIsJoinable))]
[HarmonyPriority(Priority.Last)]
[HarmonyWrapSafe]
internal static class LobbyDataIsJoinablePostfix
{
    [HarmonyPostfix]
    private static bool Postfix(bool __result, ref Lobby lobby)
    {
        // If original result was false, return false
        if (!__result)
            return false;

        // If the lobby is not modded, return original result
        if (lobby.GetData(LobbyMetadata.Modded) != "true")
            return __result;

        string lobbyPluginString = lobby.GetData(LobbyMetadata.Plugins);

        // If the lobby does not have any plugin information, return original result
        if (string.IsNullOrEmpty(lobbyPluginString))
        {
            Log.Warn("Lobby is modded but does not have any plugin information.");
            return __result;
        }

        bool matchesPluginRequirements =
            PluginHelper.MatchesTargetRequirements(PluginHelper.ParseLobbyPluginsMetadata(lobbyPluginString));

        if (!matchesPluginRequirements)
        {
            Log.Warn("You are missing required plugins to join this lobby.");
            UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(
                false,
                RoomEnter.Error,
                "You are missing required mods to join this lobby.");
        }

        return matchesPluginRequirements;
    }
}