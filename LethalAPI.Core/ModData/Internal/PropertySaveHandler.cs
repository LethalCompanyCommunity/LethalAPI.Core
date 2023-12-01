// -----------------------------------------------------------------------
// <copyright file="PropertySaveType.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;
using System.Reflection;

using Attributes;
using Interfaces;

/// <inheritdoc />
internal sealed class PropertySaveHandler : SaveHandler
{
    private readonly IPlugin<IConfig> plugin;
    private readonly PropertyInfo property;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to search.</param>
    /// <exception cref="MissingMemberException">Thrown if no valid save data property is found in the plugin.</exception>
    internal PropertySaveHandler(IPlugin<IConfig> plugin)
    {
        this.plugin = plugin;
        foreach (PropertyInfo propertyInfo in this.plugin.GetType().GetProperties())
        {
            if (propertyInfo.Name == "SaveData")
            {
                this.property = propertyInfo;
                goto ensureValid;
            }

            if (propertyInfo.GetCustomAttribute<SaveDataAttribute>() is not { })
                continue;

            this.property = propertyInfo;

            // Check to ensure the property has a getter and setter. If it doesnt, continue.
            ensureValid:
            if (this.property.GetMethod is null || this.property.SetMethod is null)
                continue;

            return;
        }

        throw new MissingMemberException("The SaveData property does not exist for this plugin! It cannot save data to or from.");
    }

    /// <inheritdoc />
    internal override ISave Save
    {
        get => (ISave)this.property.GetValue(this.plugin);
        set => this.property.SetValue(this.plugin, value);
    }
}