// -----------------------------------------------------------------------
// <copyright file="MovementController.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
namespace LethalAPI.Core.Features;

using GameNetcodeStuff;

/// <summary>
/// Contains information about the player's movement.
/// </summary>
public class MovementController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MovementController"/> class.
    /// </summary>
    /// <param name="ply">The player this instance represents.</param>
    public MovementController(Player ply)
    {
        this.Player = ply;
    }

    /// <summary>
    /// Gets the player that this class represents.
    /// </summary>
    public Player Player { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is walking.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isWalking">
    /// PlayerControllerB.isWalking
    /// </seealso>
    public bool IsWalking
    {
        get => Player.Base.isWalking;
        set => Player.Base.isWalking = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is crouching.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isCrouching">
    /// PlayerControllerB.isCrouching
    /// </seealso>
    public bool IsCrouching
    {
        get => Player.Base.isCrouching;
        set => Player.Base.isCrouching = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is sprinting.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isSprinting">
    /// PlayerControllerB.isSprinting
    /// </seealso>
    public bool IsSprinting
    {
        get => Player.Base.isSprinting;
        set => Player.Base.isSprinting = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is exhausted.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isExhausted">
    /// PlayerControllerB.isExhausted
    /// </seealso>
    public bool IsExhausted
    {
        get => Player.Base.isExhausted;
        set => Player.Base.isExhausted = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is climbing a ladder.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isClimbingLadder">
    /// PlayerControllerB.isClimbingLadder
    /// </seealso>
    public bool IsClimbingLadder
    {
        get => Player.Base.isClimbingLadder;
        set => Player.Base.isClimbingLadder = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is falling due to a jump.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isFallingFromJump">
    /// PlayerControllerB.isFallingFromJump
    /// </seealso>
    public bool IsFallingFromJump
    {
        get => Player.Base.isFallingFromJump;
        set => Player.Base.isFallingFromJump = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is falling but not due to a jump.
    /// </summary>
    /// <seealso cref="MovementController.IsFallingFromJump">
    ///     MovementController.IsFallingFromJump
    /// </seealso>
    /// <seealso cref="PlayerControllerB.isFallingNoJump">
    ///     PlayerControllerB.isFallingNoJump
    /// </seealso>
    public bool IsFalling
    {
        get => Player.Base.isFallingNoJump;
        set => Player.Base.isFallingNoJump = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is sinking.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isSinking">
    /// PlayerControllerB.isSinking
    /// </seealso>
    public bool IsSinking
    {
        get => Player.Base.isSinking;
        set => Player.Base.isSinking = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> has hindered movement.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isMovementHindered">
    /// PlayerControllerB.isMovementHindered
    /// </seealso>
    public int IsHindered
    {
        get => Player.Base.isMovementHindered;
        set => Player.Base.isMovementHindered = value;
    }

    /// <summary>
    /// Gets or sets a value representing the fall speed of the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.fallValue">
    /// PlayerControllerB.fallValue
    /// </seealso>
    public float FallSpeed
    {
        get => Player.Base.fallValue;
        set => Player.Base.fallValue = value;
    }
}