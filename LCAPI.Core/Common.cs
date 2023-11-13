// -----------------------------------------------------------------------
// <copyright file="Common.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core;

/// <summary>
/// Class that hold common/shared code for other LCAPI library
/// </summary>
public static class Common
{
    /// <summary>
    /// Get whether the object is a value type or not
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsValueType<T>(T obj)
        where T : class
    {
        return obj.GetType().IsValueType;
    }
}