// -----------------------------------------------------------------------
// <copyright file="StaticImplementation.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace LethalAPI.Core.Features;

using System.Collections.Generic;
using System.Linq;

using GameNetcodeStuff;

/// <summary>
///     Contains information about a player.
/// </summary>
public partial class Player
{
    /// <summary>
    /// Gets a list of all players.
    /// </summary>
    public static List<Player> List => new List<Player>();

    /// <summary>
    /// Returns an instance of the player.
    /// </summary>
    /// <param name="player">The player instance to get.</param>
    /// <returns>The player.</returns>
    public static Player Get(PlayerControllerB player)
    {
        if (player is null)
            return null;
        Player ply = List.FirstOrDefault(x => x.Base == player);
        if (ply is null)
        {
            ply = new Player(player);
            List.Add(ply);
        }

        return ply;
    }
}