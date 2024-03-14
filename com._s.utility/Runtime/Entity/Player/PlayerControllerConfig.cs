using System;
using _S.Utility;
using UnityEngine;
using _S.Attributes;
using _S.ScriptableVariables;

namespace _S.Configuration
{
    [CreateAssetMenu(menuName = "Config/Player/Player Controller")]
    [Serializable]
    public class PlayerControllerConfig : ScriptableObject
    {
        [SerializeField, Section("Movement and Rotation", new string[] { "MoveSpeed", "MaxSpeed", "CrouchSpeedMultiplier", "CrouchYOffset", "CrouchSpeed", "Sensitivity", "MoveAcceleration", "AirAccelerationMultiplier", "ForceDecceleration" })] bool _movementExtended;
        [Tooltip("The horizontal movement speed of the player"), HideInInspector]
        public float MoveSpeed;
        [Tooltip("The maximum horizontal speed the player can reach"), HideInInspector]
        public float MaxSpeed;
        [Tooltip("The player's horizontal crouch speed multiplier "), HideInInspector]
        public float CrouchSpeedMultiplier;
        [Tooltip("The vertical offset of the player's camera when crouched"), HideInInspector]
        public float CrouchYOffset;
        [Tooltip("The speed the player stops/starts crouching"), HideInInspector]
        public float CrouchSpeed;
        [Tooltip("The player's look sensitivity"), HideInInspector]
        public ClampedFloatReference Sensitivity;
        [Tooltip("How fast the player's movement accelerates and decelerates"), HideInInspector]
        public float MoveAcceleration;
        [Tooltip("How fast forces acting on the player decelerate"), HideInInspector]
        public float ForceDecceleration;
        [Tooltip("Acceleration multiplier in the air"), HideInInspector]
        public float AirAccelerationMultiplier;

        [SerializeField, Section("Jumping", new string[] { "JumpForceVertical", "JumpForceHorizontal", "JumpCooldown", "JumpPressBufferCooldown", "CoyoteTimeCooldown", "JumpHoldTimeCooldown", "JumpGravityMultiplier", "JumpFallGravityMultiplier", "JumpFallVelocity" })] bool _jumpingExtended;
        [Tooltip("The vertical force of the jump"), HideInInspector]
        public float JumpForceVertical;
        [Tooltip("The horizontal force of the jump"), HideInInspector]
        public float JumpForceHorizontal;
        [Tooltip("The time between jumps can occur"), HideInInspector]
        public Cooldown JumpCooldown = new();
        [Tooltip("The time the player has between pressing jump and touching the ground for the press to register"), HideInInspector]
        public Cooldown JumpPressBufferCooldown = new();
        [Tooltip("The time the player has between leaving the ground and pressing jump to jump"), HideInInspector]
        public Cooldown CoyoteTimeCooldown = new();
        [Tooltip("The time the jump button can be held for gravity to be decreased"), HideInInspector]
        public Cooldown JumpHoldTimeCooldown = new();
        [Tooltip("How multiplier to gravity when the button is held, factor moves toward zero linearly over the jump hold time"), HideInInspector]
        public float JumpGravityMultiplier;
        [Tooltip("The multiplier to gravity when the jump button is released"), HideInInspector]
        public float JumpFallGravityMultiplier;
        [Tooltip("The vertical velocity the player has to be at below the land event is called"), HideInInspector]
        public float JumpFallVelocity;

        [SerializeField, Section("Physics", new string[] { "GravityMultiplier", "EnemyPushPower", "GroundMask", "SlideAngle", "SliderFriction", "Stepheight", "MinStepDistance", "StepTransitionSpeed" })] bool _physicsExtended;
        [Tooltip("The multiplier to the default gravity of -9.81"), HideInInspector]
        public float GravityMultiplier;
        [Tooltip("How strong the player is pushed out of enemies"), HideInInspector]
        public float EnemyPushPower;
        [Tooltip("The ground layers that the player will stick to"), HideInInspector]
        public LayerMask GroundMask;
        [Tooltip("The upper angle of the ground surface where the player stops locking to the ground and starts sliding"), HideInInspector]
        public float SlideAngle;
        [Tooltip("The gravity factor of slanted surfaces the player slides on"), HideInInspector]
        public float SliderFriction;
        [Tooltip("The height that the player snaps up and down stairs and down inclines"), HideInInspector]
        public float Stepheight;
        [Tooltip("The minimum horizontal distance for the player to go up a step"), HideInInspector]
        public float MinStepDistance;
        [Tooltip("The speed the player tweens up and down steps"), HideInInspector]
        public float StepTransitionSpeed;
    }
}