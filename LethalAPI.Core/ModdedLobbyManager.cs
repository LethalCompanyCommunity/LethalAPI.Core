// -----------------------------------------------------------------------
// <copyright file="ModdedLobbyManager.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once InconsistentNaming
#pragma warning disable SA1401

/// <summary>
/// Manages the modded lobby instances.
/// </summary>
public static class ModdedLobbyManager
{
    /// <summary>
    /// The amount of times a user has been prevented from joining non-modded lobbies.
    /// </summary>
    internal static int ModdedOnlyCounter;

    /// <summary>
    /// Indicates whether the user is only allowed to join a modded lobby, or that only modded player's are allowed to join the user's lobby.
    /// </summary>
    internal static bool ModdedOnly;

    /// <summary>
    /// Manual switch for forced modded only.<br/>
    /// This use an internal counter, so you don't have to worry about you breaking anything.
    /// </summary>
    /// <param name="flag">Whether to enable or disable.</param>
    public static void ForceModdedOnly(bool flag)
    {
        if (!flag)
        {
            ModdedOnlyCounter--;
        }
        else
        {
            ModdedOnlyCounter++;
        }

        ModdedOnly = ModdedOnlyCounter > 0;
    }
}