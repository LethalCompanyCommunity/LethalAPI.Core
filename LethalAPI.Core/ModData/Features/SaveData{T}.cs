// -----------------------------------------------------------------------
// <copyright file="SaveData{T}.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.Saves.Features;

using LethalAPI.Core.ModData.Features;

/// <summary>
/// Represents data stored in the save file.
/// </summary>
/// <typeparam name="T">The type of data to save.</typeparam>
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
public class SaveData<T> : SaveData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveData{T}"/> class.
    /// </summary>
    /// <param name="prefix">The prefix of the item.</param>
    public SaveData(string prefix)
        : base(prefix, typeof(T))
    {
    }

    /// <summary>
    /// Gets or sets the value of the data.
    /// </summary>
    public new T Value
    {
        get
        {
            if (!this.loaded)
            {
                GetValue();
            }

            return (T)this.value;
        }

        set
        {
            this.value = value;
            SaveValue();
        }
    }

    /// <summary>
    /// Saves the value to the mod save file.
    /// </summary>
    protected override void SaveValue()
    {
        loaded = true;
    }

    /// <summary>
    /// Gets the value to the mod save file.
    /// </summary>
    protected override void GetValue()
    {
        loaded = true;
    }
}