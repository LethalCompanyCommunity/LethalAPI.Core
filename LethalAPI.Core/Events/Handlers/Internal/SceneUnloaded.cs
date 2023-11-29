// -----------------------------------------------------------------------
// <copyright file="SceneUnloaded.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the LGPL-3.0 license.
// </copyright>
// Taken from EXILED (https://github.com/Exiled-Team/EXILED)
// Licensed under the CC BY SA 3 license. View it here:
// https://github.com/Exiled-Team/EXILED/blob/master/LICENSE.md
// Changes: Namespace adjustments, and potential removed properties.
// -----------------------------------------------------------------------

#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

namespace LethalAPI.Core.Events.Handlers.Internal;

using UnityEngine.SceneManagement;

/// <summary>
///     Handles scene unload event.
/// </summary>
internal static class SceneUnloaded
{
    /// <summary>
    ///     Called once when the server changes the scene.
    /// </summary>
    public static void OnSceneUnloaded(Scene _)
    {
    }
}