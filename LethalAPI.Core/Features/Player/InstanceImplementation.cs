// -----------------------------------------------------------------------
// <copyright file="InstanceImplementation.cs" company="LethalAPI Modding Community">
// Copyright (c) LethalAPI Modding Community. All rights reserved.
// Licensed under the GPL-3.0 license.
// </copyright>
// -----------------------------------------------------------------------

// ReSharper disable once CheckNamespace
// ReSharper disable MemberCanBePrivate.Global
namespace LethalAPI.Core.Features;

using GameNetcodeStuff;
using UnityEngine;

/// <summary>
///     Contains information about a player.
/// </summary>
// ReSharper disable CommentTypo
public partial class Player
{
    /*-----------------------
     *
     * Cache
     *
     -----------------------*/
    // Cached Lookups are allowed in here as an exception to naming scheme.
    private static readonly int Limp = Animator.StringToHash("Limp");

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    /// <param name="basePlayer">The base player.</param>
    public Player(PlayerControllerB basePlayer)
    {
        this.Base = basePlayer;
    }

    /*-----------------------
     *
     * Properties
     *
     -----------------------*/

    /// <summary>
    /// Gets the base <see cref="PlayerControllerB"/> for the player.
    /// </summary>
    public PlayerControllerB Base { get; private set; }

    /// <summary>
    /// Gets the <see cref="MovementController"/> which contains information about player movement.
    /// </summary>
    public MovementController MovementController => new MovementController(this);

    /// <summary>
    /// Gets the <see cref="DrunkennessController"/> which contains information about player drunkenness.
    /// </summary>
    public DrunkennessController DrunkennessController => new DrunkennessController(this);

    /// <summary>
    /// Gets the <see cref="Light"/> of the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.helmetLight">
    /// PlayerControllerB.helmetLight
    /// </seealso>
    public Light HelmetLight => this.Base.helmetLight;

    /// <summary>
    /// Gets or sets the transform of the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.thisPlayerBody">
    /// PlayerControllerB.thisPlayerBody
    /// </seealso>
    public Transform Transform
    {
        get => this.Base.thisPlayerBody;
        set => this.Base.thisPlayerBody = value;
    }

    /// <summary>
    /// Gets or sets a value representing the username of the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.playerUsername">
    /// PlayerControllerB.playerUsername
    /// </seealso>
    public string Username
    {
        get => this.Base.playerUsername;
        set => this.Base.playerUsername = value;
    }

    /// <summary>
    /// Gets or sets a value representing the health of the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.health">
    /// PlayerControllerB.health
    /// </seealso>
    public int Health
    {
        get => this.Base.health;
        set => this.Base.health = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is dead.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isPlayerDead">
    /// PlayerControllerB.isPlayerDead
    /// </seealso>
    public bool IsDead
    {
        get => this.Base.isPlayerDead;
        set => this.Base.isPlayerDead = value;
    }

    /// <summary>
    /// Gets the <see cref="DeadBodyInfo"/> for the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.deadBody">
    /// PlayerControllerB.deadBody
    /// </seealso>
    public DeadBodyInfo BodyInfo => this.Base.deadBody;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is underwater.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isUnderwater">
    /// PlayerControllerB.isUnderwater
    /// </seealso>
    public bool IsUnderwater
    {
        get => this.Base.isUnderwater;
        set
        {
            this.Base.isUnderwater = value;
            this.Base.isFaceUnderwaterOnServer = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is sideways.
    /// </summary>
    /// <seealso cref="PlayerControllerB.isSidling">
    /// PlayerControllerB.isSidling
    /// </seealso>
    public bool IsSideways
    {
        get => this.Base.isSidling;
        set => this.Base.isSidling = value;
    }

    /// <summary>
    /// Gets or the <see cref="PlayerVoiceIngameSettings"/> for the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.currentVoiceChatIngameSettings">
    /// PlayerControllerB.currentVoiceChatIngameSettings
    /// </seealso>
    public PlayerVoiceIngameSettings VoiceChatSettings
        => this.Base.currentVoiceChatIngameSettings;

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="Player"/> is activating an item.
    /// </summary>
    /// <seealso cref="PlayerControllerB.activatingItem">
    /// PlayerControllerB.activatingItem
    /// </seealso>
    public bool IsActivatingItem
    {
        get => this.Base.activatingItem;
        set => this.Base.activatingItem = value;
    }

    /// <summary>
    /// Gets or sets a value representing the Carry Weight of the <see cref="Player"/> body.
    /// </summary>
    /// <seealso cref="PlayerControllerB.carryWeight">
    /// PlayerControllerB.carryWeight
    /// </seealso>
    public float CarryWeight
    {
        get => this.Base.carryWeight;
        set => this.Base.carryWeight = value;
    }

    /// <summary>
    /// Gets or sets a value representing the external forces acting on the <see cref="Player"/>.
    /// </summary>
    /// <seealso cref="PlayerControllerB.externalForces">
    /// PlayerControllerB.externalForces
    /// </seealso>
    public Vector3 ExternalForces
    {
        get => this.Base.externalForces;
        set => this.Base.externalForces = value;
    }

    /*-----------------------
     *
     * Methods
     *
     -----------------------*/

    /// <summary>
    /// Kills the player.
    /// </summary>
    /// <param name="causeOfDeath">
    ///     The <see cref="CauseOfDeath"/> for the kill.
    /// </param>
    /// <param name="spawnRagdoll">
    ///     Indicates whether a ragdoll should be spawned.
    /// </param>
    /// <param name="velocity">
    ///     The velocity of the body as it died.
    /// </param>
    /// <param name="deathAnimation">
    ///     The death animation to be played.
    /// </param>
    public void Kill(CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, bool spawnRagdoll = false, Vector3? velocity = null, int deathAnimation = 0)
        => Base.KillPlayer(velocity ?? Vector3.zero, spawnRagdoll, causeOfDeath, deathAnimation);

    /// <summary>
    /// Heals a player from a critical injury.
    /// </summary>
    public void Heal()
    {
        if (!this.Base.criticallyInjured)
        {
            return;
        }

        this.Base.criticallyInjured = false;
        this.Base.playerBodyAnimator.SetBool(Limp, false);
        this.Base.bleedingHeavily = false;
        if (this.Base.IsServer)
            this.Base.HealClientRpc();
        else
            this.Base.HealServerRpc();
    }

    /// <summary>
    /// Triggers the critically injured effect.
    /// </summary>
    public void CriticallyInjure()
    {
        if (this.Base.criticallyInjured)
        {
            return;
        }

        this.Base.criticallyInjured = true;
        this.Base.playerBodyAnimator.SetBool(Limp, true);
        this.Base.bleedingHeavily = true;
        if (this.Base.IsServer)
            this.Base.MakeCriticallyInjuredClientRpc();
        else
            this.Base.MakeCriticallyInjuredServerRpc();
    }
}
