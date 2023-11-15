// -----------------------------------------------------------------------
// <copyright file="LobbyMetadata.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
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
    ///     The tag for the lobby being modded.
    /// </summary>
    public const string Modded = "__modded";

    /// <summary>
    ///     The tag for the lobby being joinable for vanilla clients.
    /// </summary>
    public const string Joinable = "joinable";

    /// <summary>
    ///     The tag for the lobby being joinable for modded clients.
    /// </summary>
    public const string JoinableModded = "__joinable";

    /// <summary>
    ///     The tag for plugin information.
    /// </summary>
    public const string Plugins = "__plugins";
}