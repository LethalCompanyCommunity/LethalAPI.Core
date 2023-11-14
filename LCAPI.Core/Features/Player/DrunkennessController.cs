// -----------------------------------------------------------------------
// <copyright file="DrunkennessController.cs" company="Lethal Company Modding Community">
// Copyright (c) Lethal Company Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace LCAPI.Core.Features;

using GameNetcodeStuff;

/// <summary>
/// Contains information about the player's drunkenness.
/// </summary>
public class DrunkennessController
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DrunkennessController"/> class.
    /// </summary>
    /// <param name="ply">The player this is representing.</param>
    public DrunkennessController(Player ply)
    {
        this.Player = ply;
    }

    /// <summary>
    /// Gets the instance of the <see cref="Player"/> that this represents.
    /// </summary>
    public Player Player { get; }

    /// <summary>
    /// Gets or sets a value representing the level of the <see cref="Player">Player's</see> Drunkness.
    /// </summary>
    /// <seealso cref="PlayerControllerB.drunknessSpeed">
    /// PlayerControllerB.drunknessSpeed
    /// </seealso>
    public float Drunkness
    {
        get => this.Player.Base.drunkness;
        set => this.Player.Base.drunkness = value;
    }

    /// <summary>
    /// Gets or sets a value representing the inertia of the <see cref="Player">Player's</see> drunkness.
    /// </summary>
    /// <seealso cref="PlayerControllerB.drunknessInertia">
    /// PlayerControllerB.drunknessInertia
    /// </seealso>
    public float Inertia
    {
        get => this.Player.Base.drunknessInertia;
        set => this.Player.Base.drunknessInertia = value;
    }

    /// <summary>
    /// Gets or sets a value representing the speed of the <see cref="Player">Player's</see> drunkness.
    /// </summary>
    /// <seealso cref="PlayerControllerB.drunknessSpeed">
    /// PlayerControllerB.drunknessSpeed
    /// </seealso>
    public float Speed
    {
        get => this.Player.Base.drunknessSpeed;
        set => this.Player.Base.drunknessSpeed = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> will increase in drunkness this frame.
    /// </summary>
    /// <seealso cref="PlayerControllerB.increasingDrunknessThisFrame">
    /// PlayerControllerB.increasingDrunknessThisFrame
    /// </seealso>
    public bool Increasing
    {
        get => this.Player.Base.increasingDrunknessThisFrame;
        set => this.Player.Base.increasingDrunknessThisFrame = value;
    }
}