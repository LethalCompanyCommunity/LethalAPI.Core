// -----------------------------------------------------------------------
// <copyright file="PropertySaveHandler.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData.Internal;

using System;
using System.Reflection;

using Attributes;
using Interfaces;

/// <inheritdoc />
internal class PropertySaveHandler : SaveHandler
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySaveHandler"/> class.
    /// </summary>
    /// <param name="plugin">The plugin to search.</param>
    /// <exception cref="MissingMemberException">Thrown if no valid save data property is found in the plugin.</exception>
    internal PropertySaveHandler(IPlugin<IConfig> plugin)
    {
        this.Plugin = plugin;
        this.PluginInstance = plugin.RootInstance;
        foreach (PropertyInfo propertyInfo in this.Plugin.GetType().GetProperties())
        {
            if (propertyInfo.Name == "SaveData")
            {
                this.Property = propertyInfo;
                goto ensureValid;
            }

            if (propertyInfo.GetCustomAttribute<SaveDataAttribute>() is not { } settings)
                continue;

            this.Property = propertyInfo;
            this.Settings = settings;

            // Check to ensure the property has a getter and setter. If it doesnt, continue.
            ensureValid:
            if (this.Property.GetMethod is null || this.Property.SetMethod is null)
                continue;

            return;
        }

        throw new MissingMemberException("The SaveData property does not exist for this plugin!");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertySaveHandler"/> class.
    /// </summary>
    protected PropertySaveHandler()
    {
    }

    /// <inheritdoc />
    public override ISave Save
    {
        get => (ISave)this.Property.GetValue(this.PluginInstance);
        set => this.Property.SetValue(this.PluginInstance, value);
    }

    /// <summary>
    /// Gets the plugin instance. This may not be a <see cref="IPlugin{TConfig}"/>.
    /// </summary>
    protected object PluginInstance { get; init; } = null!;

    /// <summary>
    /// Gets the property instance.
    /// </summary>
    protected PropertyInfo Property { get; init; } = null!;
}