// -----------------------------------------------------------------------
// <copyright file="SteamMatchmakingOnLobbyCreatedPostfix.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Patches.Features;

using System.Linq;

using Core.Features;
using Models;
using Steamworks;
using Steamworks.Data;

/// <summary>
///     Patches <see cref="GameNetworkManager.SteamMatchmaking_OnLobbyCreated" />.
///     Adds extra lobby metadata to be used by the API for dependency checking.
/// </summary>
/// <seealso cref="GameNetworkManager.SteamMatchmaking_OnLobbyCreated" />
// ReSharper disable UnusedMember.Local
[HarmonyPatch(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyCreated")]
[HarmonyPriority(Priority.Last)]
[HarmonyWrapSafe]
internal static class SteamMatchmakingOnLobbyCreatedPostfix
{
    [HarmonyPostfix]
    private static void Postfix(Result result, ref Lobby lobby)
    {
        // lobby has not yet been created or something went wrong
        if (result != Result.OK)
            return;

        // Set up metadata for modded lobby
        lobby.SetData(LobbyMetadata.Modded, "true");
        lobby.SetData(LobbyMetadata.Plugins, PluginHelper.GetLobbyPluginsMetadata());
        lobby.SetData(LobbyMetadata.JoinableModded, lobby.GetData(LobbyMetadata.Joinable));
        lobby.SetData(LobbyMetadata.Name, "modded // " + lobby.GetData(LobbyMetadata.Name));

        Log.Debug($"Lobby plugins metadata: {PluginHelper.GetLobbyPluginsMetadata()}");

        if (PluginHelper.GetAllRequiredPluginInfo().Any())
        {
            Log.Warn("You are hosting a lobby with required plugins. Disabling vanilla clients from joining.");
            lobby.SetData(LobbyMetadata.Joinable, "false");
        }
    }
}