// -----------------------------------------------------------------------
#pragma warning disable SA1636
// <copyright file="SafeCoroutine.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the MIT License. License:
// https://github.com/o5zereth/ZerethAPI/blob/master/LICENSE.txt
// Made by O5Zereth (an absolute legend). Check out his work here:
// https://github.com/o5zereth/ZerethAPI/
// </copyright>
// -----------------------------------------------------------------------
namespace LethalAPI.Core.MEC;

using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable SA1649

/// <summary>
/// Supports a simple safe iteration over a generic collection, with methods to catch exceptions.
/// </summary>
/// <typeparam name="T">The type of objects to enumerate.</typeparam>
// ReSharper disable InconsistentNaming
public readonly struct SafeEnumerator<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> enumerator;
    private readonly Action<Exception> onException;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeEnumerator{T}"/> struct.
    /// </summary>
    /// <exception cref="InvalidOperationException">Use of a parameterless constructor is not valid for this object.</exception>
    public SafeEnumerator() =>
        throw new InvalidOperationException("Use of a parameterless constructor is not valid for this object.");

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeEnumerator{T}"/> struct.
    /// </summary>
    /// <param name="enumerator">The original enumerator to enumerate safely.</param>
    /// <param name="onException">The action to call upon an exception being thrown.</param>
    /// <exception cref="ArgumentNullException">Enumerator cannot be null.</exception>
    public SafeEnumerator(IEnumerator<T> enumerator, Action<Exception>? onException = null)
    {
        this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        this.onException = onException ?? DoNothing;
    }

    /// <inheritdoc/>
    public readonly T Current => this.enumerator.Current;

    /// <inheritdoc/>
    readonly object IEnumerator.Current => this.Current!;

    /// <inheritdoc/>
    public void Dispose() => this.enumerator.Dispose();

    /// <inheritdoc/>
    public bool MoveNext()
    {
        bool result = false;

        try
        {
            result = this.enumerator.MoveNext();
        }
        catch (Exception e)
        {
            this.onException.Invoke(e);
        }

        return result;
    }

    /// <inheritdoc/>
    public void Reset() => this.enumerator.Reset();

    private static void DoNothing(Exception e)
    {
    }
}

/// <summary>
/// Supports a simple safe iteration over a generic collection.
/// </summary>
/// <typeparam name="T">The type of objects to enumerate.</typeparam>
/// <remarks>When an exception is thrown within the underlying enumerator, it is caught, ignored, and the enumerator stops iterating.</remarks>
public readonly struct SafeEnumeratorIgnoreException<T> : IEnumerator<T>
{
    private readonly IEnumerator<T> enumerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeEnumeratorIgnoreException{T}"/> struct.
    /// </summary>
    /// <exception cref="InvalidOperationException">Use of a parameterless constructor is not valid for this object.</exception>
    public SafeEnumeratorIgnoreException() =>
        throw new InvalidOperationException("Use of a parameterless constructor is not valid for this object.");

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeEnumeratorIgnoreException{T}"/> struct.
    /// </summary>
    /// <param name="enumerator">The original enumerator to enumerate safely.</param>
    /// <exception cref="ArgumentNullException">Enumerator cannot be null.</exception>
    public SafeEnumeratorIgnoreException(IEnumerator<T> enumerator)
    {
        this.enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
    }

    /// <inheritdoc/>
    public readonly T Current => this.enumerator.Current;

    /// <inheritdoc/>
    readonly object IEnumerator.Current => this.Current!;

    /// <inheritdoc/>
    public void Dispose() => this.enumerator.Dispose();

    /// <inheritdoc/>
    public bool MoveNext()
    {
        bool result = false;

        try
        {
            result = this.enumerator.MoveNext();
        }
        catch
        {
            // ignored
        }

        return result;
    }

    /// <inheritdoc/>
    public void Reset() => this.enumerator.Reset();
}