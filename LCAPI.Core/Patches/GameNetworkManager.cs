// -----------------------------------------------------------------------
// <copyright file="GameNetworkManager.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Patches;

using HarmonyLib;
using Steamworks;
using Steamworks.Data;
using System.Linq;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManagerPatches
{

    [HarmonyPatch("SteamMatchmaking_OnLobbyCreated")]
    [HarmonyPostfix]
    [HarmonyWrapSafe]
    private static void SteamMatchmaking_OnLobbyCreated_Postfix(Result result, ref Lobby lobby)
    {
        // lobby has not yet created or something went wrong
        if (result != Result.OK)
        {
            return;
        }

        lobby.SetData("__modded_user", "true");
        lobby.SetData("__joinable", lobby.GetData("joinable"));
        lobby.SetData("joinable", "false");
    }

    [HarmonyPatch(nameof(GameNetworkManager.LobbyDataIsJoinable))]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    private static bool LobbyDataIsJoinable_Prefix(GameNetworkManager __instance, ref Lobby lobby, ref bool __result)
    {
        var data = lobby.GetData("__modded_user");
        if (data != "true")
        {
            UnityEngine.Debug.Log("Lobby join denied! Attempted to join non-modded lobby");
            UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(false, RoomEnter.DoesntExist, "The server host is not a modded user");
            __result = false;
            return false;
        }
        data = lobby.GetData("vers");
        if (data != __instance.gameVersionNum.ToString())
        {
            UnityEngine.Debug.Log(string.Format("Lobby join denied! Attempted to join vers.{0} lobby id: {1}", data, lobby.Id));
            UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(false, RoomEnter.DoesntExist, string.Format("The server host is playing on version {0} while you are on version {1}.", data, GameNetworkManager.Instance.gameVersionNum));
            __result = false;
            return false;
        }

        Friend[] friendArr = SteamFriends.GetBlocked().ToArray<Friend>();
        if (friendArr != null && friendArr.Length > 0)
        {
            foreach (var friend in friendArr)
            {
                UnityEngine.Debug.Log(string.Format("blocked users: name: {0} | id: {1}", friend.Name, friend.Id));
                if (lobby.IsOwnedBy(friend.Id))
                {
                    UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(false, RoomEnter.DoesntExist, "An error occured!");
                    __result = false;
                    return false;
                }
            }
        }
        data = lobby.GetData("__joinable");
        if (data == "false")
        {
            UnityEngine.Debug.Log("Lobby join denied! Host lobby is not joinable");
            UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(false, RoomEnter.DoesntExist, "The server host has already landed their ship, or they are still loading in.");
            return false;
        }

        if (lobby.MemberCount >= 4 || lobby.MemberCount < 1)
        {
            UnityEngine.Debug.Log(string.Format("Lobby join denied! Too many members in lobby! {0}", lobby.Id));
            UObject.FindObjectOfType<MenuManager>().SetLoadingScreen(false, RoomEnter.Full, "The server is full!");
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }
}