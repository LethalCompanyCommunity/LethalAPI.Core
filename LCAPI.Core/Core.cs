// -----------------------------------------------------------------------
// <copyright file="Core.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core;

public static class Core
{
    internal static int _moddedOnlyCounter = 0;
    internal static bool _moddedOnly = false;

    /// <summary>
    /// Manual switch for forced modded only.<br/>
    /// This use an internal counter, so you don't have to worry about you breaking anything.
    /// </summary>
    /// <param name="flag">Whether to enable or disable.</param>
    public static void ForceModdedOnly(bool flag)
    {
        if (!flag)
        {
            _moddedOnlyCounter--;
        }
        else
        {
            _moddedOnlyCounter++;
        }

        _moddedOnly = _moddedOnlyCounter > 0;
    }
}