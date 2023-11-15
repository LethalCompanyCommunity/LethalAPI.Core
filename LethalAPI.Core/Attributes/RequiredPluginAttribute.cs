// -----------------------------------------------------------------------
// <copyright file="RequiredPluginAttribute.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Attributes;

using System;

/// <summary>
///     Specifies that a plugin is required for all players to have installed.
/// </summary>
public class RequiredPluginAttribute
{
    /// <summary>
    ///     Specifies that a plugin is required for all players to have installed.
    /// </summary>
    public sealed class PluginRequired : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginRequired" /> class.
        /// </summary>
        /// <param name="guid"> The GUID of the plugin. </param>
        public PluginRequired(string guid)
        {
            Guid = guid;
            PluginManager.AddRequiredPluginGuid(guid);
        }

        /// <summary>
        ///     Gets the GUID of the plugin.
        /// </summary>
        public string Guid { get; }
    }
}