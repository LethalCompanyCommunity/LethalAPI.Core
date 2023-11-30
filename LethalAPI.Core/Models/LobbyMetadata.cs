// -----------------------------------------------------------------------
// <copyright file="LobbyMetadata.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace LethalAPI.Core.Models;

/// <summary>
///     Metadata tags for use in the lobby manager.
/// </summary>
public static class LobbyMetadata
{
    /// <summary>
    ///     The tag for the lobby name.
    /// </summary>
    public const string Name = "name";

    /// <summary>
    ///     The tag for the host's game version.
    /// </summary>
    /// <remarks>
    ///     Users who try to join compare against this. Mismatches cause a failure to join.
    ///     This should generally not be overridden.
    /// </remarks>
    public const string Version = "vers";

    /// <summary>
    ///     The tag for the lobby being joinable for vanilla clients.
    /// </summary>
    public const string Joinable = "joinable";

    /// <summary>
    ///     The tag for the lobby being modded.
    /// </summary>
    public const string Modded = "__modded";

    /// <summary>
    ///     The tag for the lobby being joinable for modded clients.
    /// </summary>
    public const string JoinableModded = "__joinable";

    /// <summary>
    ///     The tag for plugin information.
    /// </summary>
    public const string Plugins = "__plugins";
}