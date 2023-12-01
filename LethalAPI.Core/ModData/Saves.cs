// -----------------------------------------------------------------------
// <copyright file="Saves.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LethalAPI.Core.ModData;

using System;
using System.Collections.Generic;
using System.IO;
using LethalAPI.Core.Loader;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Contain internal code for the system to work.
/// </summary>
internal sealed class Saves
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Saves"/> class.
    /// </summary>
    internal Saves()
    {
        if (Instance is not null)
        {
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Gets the instance of this class.
    /// </summary>
    internal static Saves? Instance { get; private set; }


    /// <summary>
    /// Load the mod save, DO NOT CALL MORE THAN ONCE.
    /// </summary>
    internal void LoadSaveData()
    {
        string savePath = Path.Combine(UnityEngine.Application.persistentDataPath, "ModSaveData");
        int saveNumber = Convert.ToInt32(GameNetworkManager.Instance.currentSaveFileName.Substring(GameNetworkManager.Instance.currentSaveFileName.Length - 1, 1));
    }
}
