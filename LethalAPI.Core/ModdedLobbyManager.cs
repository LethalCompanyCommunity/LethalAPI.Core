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
    /// The count of mods installed.
    /// </summary>
    internal static int ModdedOnlyCounter;

    /// <summary>
    /// Indicates whether only modded players are allowed on the server.
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