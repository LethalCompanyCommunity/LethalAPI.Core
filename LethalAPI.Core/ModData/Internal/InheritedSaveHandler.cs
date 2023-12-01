// -----------------------------------------------------------------------
// <copyright file="InheritedSaveType.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;
using System.Linq;
using System.Reflection;

using LethalAPI.Core.Interfaces;

/// <inheritdoc />
internal sealed class InheritedSaveHandler : SaveHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InheritedSaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to get the save instance from.</param>
    internal InheritedSaveHandler(IPlugin<IConfig> plugin)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (plugin is not ISavePlugin { } save)
        {
            throw new ArgumentNullException(nameof(plugin), "Plugin does not inherit ISavePlugin.");
        }

        this.SaveInstance = save;
    }

    /// <inheritdoc />
    internal override ISave Save
    {
        get => this.SaveInstance.SaveData;
        set => this.SaveInstance.SaveData = value;
    }

    private ISavePlugin SaveInstance { get; init; }
}