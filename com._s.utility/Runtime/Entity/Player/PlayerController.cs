using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using _S.Attributes;
using _S.Configuration;
using _S.ScriptableVariables;
using _S.Utility;

namespace _S.Entity.Player
{
    [RequireComponent(typeof(CharacterController)), DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] PlayerControllerConfig _config;

        public PlayerControllerConfig config
        {
            get
            {
                if (_config == null)
                {
                    _config = ScriptableObject.CreateInstance("PlayerControllerConfig") as PlayerControllerConfig;
                    Debug.LogWarning($"{this} has been disabled because it does not have a PlayerControllerConfig." +
                        $"An empty config has been assigned to prevent errors");
                    this.enabled = false;
                }
                return _config;
            }
        }

        [SerializeField, Section("Events", new string[] { "SetToTransform", "TouchingGround", "LeavingGround", "StartJump", "FallLand" })] bool _eventsExtended;
        [HideInInspector] public UnityEvent SetToTransform;
        [HideInInspector] public UnityEvent TouchingGround;
        [HideInInspector] public UnityEvent LeavingGround;
        [HideInInspector] public UnityEvent StartJump;
        [HideInInspector] public UnityEvent FallLand;

        [SerializeField, Section("References", new string[] { "_playerCamera", "_normalDummy", "_hurtbox", "_playerReference", "_cameraReference" })] bool _referencesExtended;
        [SerializeField, HideInInspector] Camera _playerCamera;
        [SerializeField, HideInInspector] Transform _normalDummy;
        [SerializeField, HideInInspector] CapsuleCollider _hurtbox;
        [SerializeField, HideInInspector] TransformVariable _playerReference;
        [SerializeField, HideInInspector] TransformVariable _cameraReference;
        CharacterController _controller;

        [SerializeField, Section("Controls", new string[] { "_moveAction", "_mouseDeltaAction", "_jumpAction", "_crouchAction" })] bool _controlsExtended;
        [SerializeField, HideInInspector] InputActionReference _moveAction;
        [SerializeField, HideInInspector] InputActionReference _mouseDeltaAction;
        [SerializeField, HideInInspector] InputActionReference _jumpAction;
        [SerializeField, HideInInspector] InputActionReference _crouchAction;

        [SerializeField, Section("Action Enablers", new string[] { "_allowJump", "_allowMove", "_allowLook", "_allowCrouch" })] bool _ActionEnablersExtended;
        [SerializeField, HideInInspector] bool _allowJump = true;
        [SerializeField, HideInInspector] bool _allowMove = true;
        [SerializeField, HideInInspector] bool _allowLook = true;
        [SerializeField, HideInInspector] bool _allowCrouch = true;

        [SerializeField, Section("Debug", new string[]
        {
        "_movement",
        "_movementAngleAdjusted",
        "_horizontalVelocity",
        "_verticalVelocity",
        "_lastGround",
        "_activeGlobalPlatformPoint",
        "_activeLocalPlatformPoint",
        "_lastGroundPosition",
        "_moveDirection",
        "_fallPeak",
        "_isJumping",
        "_isJumpFalling",
        "_isCrouching",
        "_crouchOffset",
        "_initialCameraY",
        "_initialControllerHeight",
        "_initialControllerYCenter",
        "_roofColliders",
        "_groundColliders",
        "_onSlideSurface",
        "_controllerHitNormal",
        "_controllerHitAngle" })]
        bool _debugExtended;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _movement;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _movementAngleAdjusted;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _horizontalVelocity;
        public Vector3 horizontalVelocity
        {
            get { return _horizontalVelocity; }
            set { _horizontalVelocity = value; }
        }

        [SerializeField, HideInInspector, ReadOnly] float _verticalVelocity;

        [SerializeField, HideInInspector, ReadOnly] Transform _lastGround;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _lastGroundPosition;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _activeGlobalPlatformPoint;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _activeLocalPlatformPoint;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _moveDirection;
        [SerializeField, HideInInspector, ReadOnly] float _fallPeak;

        [SerializeField, HideInInspector, ReadOnly] bool _isJumping;
        [SerializeField, HideInInspector, ReadOnly] bool _isJumpFalling;

        [SerializeField, HideInInspector, ReadOnly] bool _isCrouching;
        [SerializeField, HideInInspector, ReadOnly] bool _crouchBuffered;
        [SerializeField, HideInInspector, ReadOnly] float _crouchOffset;

        [SerializeField, HideInInspector, ReadOnly] float _initialCameraY;
        [SerializeField, HideInInspector, ReadOnly] float _initialControllerHeight;
        [SerializeField, HideInInspector, ReadOnly] float _initialControllerYCenter;

        [SerializeField, HideInInspector, ReadOnly] int _roofColliders;
        [SerializeField, HideInInspector, ReadOnly] int _groundColliders;
        [SerializeField, HideInInspector, ReadOnly] bool _onSlideSurface;

        public bool touchingRoof => _roofColliders > 0;
        public bool isGrounded => _groundColliders > 0 && !_onSlideSurface;

        [SerializeField, HideInInspector, ReadOnly] Vector3 _controllerHitNormal;
        [SerializeField, HideInInspector, ReadOnly] float _controllerHitAngle;

        private void Awake()
        {
            _playerReference.value = transform;
            _cameraReference.value = _playerCamera.transform;
            SetToTransform.Invoke();
        }

        // Start is called before the first frame update
        void Start()
        {
            _roofColliders = 0;
            _groundColliders = 0;
            _isJumping = false;
            _isJumpFalling = false;
            _isJumping = false;
            _isCrouching = false;
            _onSlideSurface = false;

            _controller = gameObject.GetComponent<CharacterController>();
            config.JumpCooldown.Initialize();
            config.JumpPressBufferCooldown.Initialize();
            config.CoyoteTimeCooldown.Initialize();
            config.JumpHoldTimeCooldown.Initialize();

            config.JumpCooldown.completed += (x) => { if (!config.JumpHoldTimeCooldown.active) _isJumping = false; };

            config.JumpHoldTimeCooldown.completed += OnJumpReleased;
            TouchingGround.AddListener(CheckJumpBuffer);
            LeavingGround.AddListener(() => { if (_verticalVelocity <= 0) { config.CoyoteTimeCooldown.Start(); } });

            _jumpAction.action.performed += OnJumpPressed;
            _jumpAction.action.canceled += OnJumpReleased;

            _crouchAction.action.performed += OnCrouchPressed;
            _crouchAction.action.canceled += OnCrouchReleased;
            _initialCameraY = _playerCamera.transform.localPosition.y;
            _initialControllerHeight = _controller.height;
            _initialControllerYCenter = _controller.center.y;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
        {
            GroundLock();
            if (_allowMove) { Movement(); }
            Gravity();

            // Moves the player controller using the horizontal and vertical velocity
            // If the player is sliding, apply vertical forces along the ground
            if (_onSlideSurface)
            {
                Vector3 slideDirection = Vector3.Cross(Vector3.Cross(_controllerHitNormal, Vector3.up), _controllerHitNormal).normalized;
                _controller.Move(slideDirection * _verticalVelocity / 60);
                _controller.Move(_horizontalVelocity * Time.deltaTime);
            }
            else
            {
                _controller.Move((_horizontalVelocity + Vector3.up * _verticalVelocity) / 60);
            }
            _controller.detectCollisions = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (_allowLook) { Rotation(); }
            Crouching();
        }

        // Rotates a dummy object in the direction of the normal of the ground,
        // this allows the player's movement to be translated across this axis
        void GroundLock()
        {
            // If there are no ground colliders, don't lock
            Debug.DrawRay(_normalDummy.position, Vector3.down * _normalDummy.localScale.y, Color.blue);
            if (Physics.Raycast(_normalDummy.position, Vector3.down * _normalDummy.localScale.y, out RaycastHit hit, _normalDummy.localScale.y, _config.GroundMask, QueryTriggerInteraction.Ignore))
            {
                if (_groundColliders == 0)
                {
                    _onSlideSurface = _controllerHitAngle > config.SlideAngle && _controllerHitAngle < 89;
                    _normalDummy.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                }

                if (_lastGround == hit.collider.transform)
                {
                    Vector3 newGlobalPlatformPoint = _lastGround.TransformPoint(_activeLocalPlatformPoint);
                    _moveDirection = hit.collider.transform.position - _lastGroundPosition;
                    Vector3 tempPos = transform.position;
                    //transform.position += _moveDirection;
                    _controller.Move(_moveDirection);

                    _lastGroundPosition = hit.collider.transform.position;

                    _activeGlobalPlatformPoint = transform.position;
                    _activeLocalPlatformPoint = _lastGround.InverseTransformPoint(transform.position);
                }
                else
                {
                    _lastGround = hit.collider.transform;
                    _lastGroundPosition = hit.collider.transform.position; 
                    _activeGlobalPlatformPoint = transform.position;
                    _activeLocalPlatformPoint = _lastGround.InverseTransformPoint(transform.position);
                }
            }
            else
            {
                _normalDummy.eulerAngles = Vector3.zero;
                _onSlideSurface = false;
                _lastGround = null;
            }

            if (_groundColliders > 0)
            {
                // Check if the player is colliding with a slidable surface
                _onSlideSurface = _controllerHitAngle > config.SlideAngle && _controllerHitAngle < 89;
                _normalDummy.rotation = Quaternion.FromToRotation(Vector3.up, _controllerHitNormal);
            }
        }

        // Moves the player based in the move input using the horizontal velocity vector
        void Movement()
        {
            float crouchMultiplier = _isCrouching ? config.CrouchSpeedMultiplier : 1;

            Vector3 moveVector = _moveAction.action.ReadValue<Vector2>();
            moveVector = new Vector3(moveVector.x, 0, moveVector.y) * crouchMultiplier;
            // If on the ground, dont apply the air acceleration multiplier
            float airMultiplier = isGrounded ? 1 : config.AirAccelerationMultiplier;
            // Moves accelerates the current move velocity towards the desired one
            _movement = Vector3.MoveTowards(
                _movement,
                transform.TransformDirection(moveVector * config.MoveSpeed),
                config.MoveAcceleration * config.MoveSpeed * airMultiplier / 60);

            // Translates the movemement vector to the ground normal
            Vector3 lastMove = _movementAngleAdjusted;
            _movementAngleAdjusted = _onSlideSurface ? _movement : _normalDummy.TransformDirection(_movement);


            // Moves the horizontal velocity towards the adjusted movement, decelerating any forces applied on the movement 
            _horizontalVelocity = Vector3.MoveTowards(
                _horizontalVelocity , _movementAngleAdjusted,
                config.ForceDecceleration * airMultiplier / 60 + Vector3.Distance(lastMove, _movementAngleAdjusted));
            _horizontalVelocity = Vector3.ClampMagnitude(_horizontalVelocity, config.MaxSpeed);
        }

        void Rotation() // Rotates the player and the camera based on the mouse delta
        {
            Vector2 lookDelta = _mouseDeltaAction.action.ReadValue<Vector2>();
            lookDelta *= config.Sensitivity.current / 100;

            // Rotates the player's Y axis, and the camera's X axis, based on the look delta
            transform.Rotate(Vector3.up * lookDelta.x);
            _playerCamera.transform.Rotate(Vector3.right * -lookDelta.y);

            // Clamps the view angle to straight up and straight down
            if (_playerCamera.transform.localEulerAngles.y == 180)
            {
                if (_playerCamera.transform.localEulerAngles.x >= 180)
                    _playerCamera.transform.localEulerAngles = new Vector3(270, 0, 0);
                else
                    _playerCamera.transform.localEulerAngles = new Vector3(90, 0, 0);
            }
        }

        // Applies gravity to the player
        void Gravity()
        {
            // Decreases the power of gravity while the jump button is held, decrease in effectiveness as the button is held
            float jumpMultiplier = _isJumping ? config.JumpGravityMultiplier * config.JumpHoldTimeCooldown.progress / config.JumpHoldTimeCooldown.Max : 1;
            if (_isJumpFalling) jumpMultiplier *= config.JumpFallGravityMultiplier;
            // If touching the ground and haven't just started jumping, apply normal gravity, else set velocity to a small negative number
            if (true) {}
            if ((!isGrounded || _verticalVelocity > 0))
            {

                // Applies friction if sliding
                float friction = _onSlideSurface ? config.SliderFriction : 1;
                _verticalVelocity += -9.81f * config.GravityMultiplier * jumpMultiplier * friction / 60;
                if (_verticalVelocity > 100) { _verticalVelocity = 100; }
            }
            else
            {
                _verticalVelocity = -0.1f * config.GravityMultiplier;
            }
        }

        // Triggers whenever the jump button is pressed
        void OnJumpPressed(InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            if (!_isJumping) { config.JumpHoldTimeCooldown.Start(); }

            // Jump if coyote time is active or you have fallen to the ground, and the jump isn't on cooldown.
            // Otherwise buffer the button press
            if (((isGrounded && _verticalVelocity <= 0) || config.CoyoteTimeCooldown.active) && !config.JumpCooldown.active)
            {
                DoJump();
            }
            else
            {
                config.JumpPressBufferCooldown.Start();
            }
        }

        // Triggers whenever the jump button is released or the jump hold time runs out
        void OnJumpReleased(InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            if (_isJumping) { _isJumpFalling = true; }
            config.JumpHoldTimeCooldown.End();
            if (config.JumpCooldown.active) { return; }
            _isJumping = false;
        }

        // Triggers when the player touches the ground and checks if a buffered jump can occur
        void CheckJumpBuffer()
        {
            if (!config.JumpCooldown.active)
            {
                config.CoyoteTimeCooldown.End();
                // Jump if the jump is buffered and not already going up
                if (config.JumpPressBufferCooldown.active && _verticalVelocity <= 0)
                {
                    config.JumpPressBufferCooldown.End();
                    // If you aren't holding the jump button, do the smallest jump
                    if (!config.JumpHoldTimeCooldown.active)
                    {
                        DoJump();
                        _isJumping = false;
                    }
                    else
                    {
                        DoJump();
                    }
                }
            }
        }

        // Apply the vertical and horizontal velocity to the player and start the jump cooldown
        void DoJump()
        {
            if (!_allowJump) { return; } 
            _isJumping = true;
            OnCrouchReleased();
            config.CoyoteTimeCooldown.End();
            config.JumpCooldown.Start();
            StartJump.Invoke();

            _verticalVelocity = Mathf.Sqrt(config.JumpForceVertical * 9.81f * config.GravityMultiplier * 2);
            _horizontalVelocity += Vector3.ClampMagnitude(_movement, 1) * config.JumpForceHorizontal;
        }

        void OnCrouchPressed(InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            _crouchBuffered = false;
            if (!isGrounded) return;
            _isCrouching = true;
        }

        void OnCrouchReleased(InputAction.CallbackContext context = new InputAction.CallbackContext())
        {
            if (touchingRoof)
            {
                _crouchBuffered = true;
                return;
            }
            _isCrouching = false;
        }

        void Crouching()
        {
            float crouchPosition = _isCrouching && _allowCrouch ? config.CrouchYOffset : 0;
            if (_crouchOffset == crouchPosition) return;
            _crouchOffset = Mathf.Lerp(_crouchOffset, crouchPosition, config.CrouchSpeed * Time.deltaTime);
            _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x, _initialCameraY + _crouchOffset, _playerCamera.transform.localPosition.z);
            _controller.height = _initialControllerHeight + _crouchOffset;
            _controller.center = new Vector3(_controller.center.x, _initialControllerYCenter + _crouchOffset / 2, _controller.center.z);
            if (_hurtbox != null)
            {
                _hurtbox.height = _initialControllerHeight + _crouchOffset;
                _hurtbox.center = new Vector3(_controller.center.x, _initialControllerYCenter + _crouchOffset / 2, _controller.center.z);
            }
        }

        // Runs whenever OnTriggerEnter is called on the ground collider
        public void GroundEnter()
        {
            if (!isGrounded)
            {
                TouchingGround.Invoke();
                _isJumpFalling = false;
                if (_verticalVelocity < config.JumpFallVelocity)
                {
                    _fallPeak = 0;
                    FallLand.Invoke();
                }
            }
            _groundColliders++;
        }

        // Runs whenever OnTriggerExit is called on the ground collider
        public void GroundExit()
        {
            _groundColliders--;
            if (!isGrounded)
            {
                LeavingGround.Invoke();
                _fallPeak = transform.position.y;
            }
        }

        // Resets gravity if the player hits their head on a ceiling
        public void RoofEnter()
        {
            _roofColliders++;
            _verticalVelocity = 0;
            _isJumping = false;
            config.JumpHoldTimeCooldown.End();
        }

        // Resets gravity if the player hits their head on a ceiling
        public void RoofExit()
        {
            _roofColliders--;
            if (_crouchBuffered)
            {
                OnCrouchReleased();
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            _controllerHitNormal = hit.normal;
            _controllerHitAngle = Vector3.Angle(_controllerHitNormal, Vector3.up);
        }
    }
}