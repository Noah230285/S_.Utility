using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using _S.Attributes;
using _S.Configuration;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using _S.Utility;
using _S.ScriptableVariables;

namespace _S.Entity.Player
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class KineticPlayerController : MonoBehaviour
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

        [SerializeField, Section("Events", new string[] { "SetToTransform", "TouchingGround", "LeavingGround" })] bool _eventsExtended;

        [HideInInspector] public UnityEvent SetToTransform;
        [HideInInspector] public UnityEvent TouchingGround;
        [HideInInspector] public UnityEvent LeavingGround;

        [SerializeField, Section("References", new string[] { "_playerCamera", "_normalDummy", "_hurtbox", "_playerReference", "_cameraReference" })] bool _referencesExtended;
        [SerializeField, HideInInspector] Camera _playerCamera;
        [SerializeField, HideInInspector] Transform _normalDummy;
        [SerializeField, HideInInspector] CapsuleCollider _hurtbox;
        [SerializeField, HideInInspector] TransformVariable _playerReference;
        [SerializeField, HideInInspector] TransformVariable _cameraReference;
        Rigidbody _rb;
        CapsuleCollider _capsule;
        public CapsuleCollider capsule => _capsule == null ? GetComponent<CapsuleCollider>() : _capsule;

        [SerializeField, Section("Controls", new string[] { "_playerInputAsset", "_moveAction", "_mouseDeltaAction", "_jumpAction", "_crouchAction" })] bool _controlsExtended;
        [SerializeField, HideInInspector] InputActionAsset _playerInputAsset;
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
        "_movementVisualisationExtended",
        "_movement",
        "_movementAngleAdjusted",
        "_horizontalVelocity",
        "_verticalVelocity",
        "_lastFloorNormal",
        "_trueVelocity",
        "_speed",
        "_trueHorizontalSpeed",
        "_trueVerticalSpeed",
        "_lastGround",
        "_activeGlobalPlatformPoint",
        "_activeLocalPlatformPoint",
        "_lastGroundPosition",
        "_moveDirection",
        "_isJumping",
        "_isJumpFalling",
        "_isCrouching",
        "_crouchOffset",
        "_initialCameraY",
        "_initialControllerHeight",
        "_initialControllerYCenter",
        "_roofColliders",
        "_groundColliders",
        "_touchingRoof",
        "_onSlideSurface",
        "_isGrounded",
        "_controllerHitNormal",
        "_controllerHitAngle" })]
        bool _DebugExtended;

        [SerializeField, Section("Movement Visualisation", new string[]
        {
        "_enableMovePreview",
        "_castDisplacement",
        "_maxDebugBounces",
        "_currentDebugBounces",
        "_showHitPoints"
        }), HideInInspector]
        bool _movementVisualisationExtended;

        [SerializeField, HideInInspector] bool _enableMovePreview;
        public bool enableMovePreview => _enableMovePreview;

        [SerializeField, HideInInspector] Vector3 _castDisplacement = Vector3.forward * 5;
        public Vector3 castDisplacement => _castDisplacement;
        [SerializeField, HideInInspector] int _maxDebugBounces = 3;
        public int maxDebugBounces => _maxDebugBounces;
        [SerializeField, HideInInspector, ReadOnly] int _currentDebugBounces = 3;
        public int currentDebugBounces
        {
            get { return _currentDebugBounces; }
            set { _currentDebugBounces = value; }
        }
        [SerializeField, HideInInspector] bool _showHitPoints;
        public bool showHitPoints => _showHitPoints;

        [SerializeField, HideInInspector, ReadOnly] Vector3 _movement;
        public Vector3 movement => _movement;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _horizontalVelocity;
        public Vector3 horizontalVelocity => _horizontalVelocity;
        [SerializeField, HideInInspector, ReadOnly] float _verticalVelocity;
        public float verticalVelocity => _verticalVelocity;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _trueVelocity;
        public Vector3 trueVelocity => _trueVelocity;
        [SerializeField, HideInInspector, ReadOnly] float _trueHorizontalSpeed;
        [SerializeField, HideInInspector, ReadOnly] float _trueVerticalSpeed;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _lastFloorNormal;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _movementAngleAdjusted;

        [SerializeField, HideInInspector, ReadOnly] float _speed;
        public float speed => _speed;
        [SerializeField, HideInInspector, ReadOnly] Transform _lastGround;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _lastGroundPosition;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _activeGlobalPlatformPoint;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _activeLocalPlatformPoint;
        [SerializeField, HideInInspector, ReadOnly] Vector3 _moveDirection;

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

        [SerializeField, HideInInspector, ReadOnly] bool _touchingRoof;
        public bool touchingRoof => _touchingRoof;
        [SerializeField, HideInInspector, ReadOnly] bool _isGrounded;
        public bool isGrounded => _isGrounded;
        public Vector3 colliderCenter => transform.lossyScale.y * capsule.center;

        [SerializeField, HideInInspector, ReadOnly] Vector3 _controllerHitNormal;
        [SerializeField, HideInInspector, ReadOnly] float _controllerHitAngle;

        public float skinWidth => capsule.radius * 0.03f;

        private void Awake()
        {
            _playerReference.value = transform;
            _playerReference.value = _playerCamera.transform;
            SetToTransform.Invoke();
            PlayerInput[] pis = FindObjectsOfType(typeof(PlayerInput)) as PlayerInput[];
            if (pis.Length == 0)
            {
                Debug.LogWarning("player input automatically added to scene, please add one to this scene once out of play mode");
                GameObject obj = new("Player Input (automatically added)");
                var playerInput = obj.AddComponent<PlayerInput>();
                playerInput.actions = _playerInputAsset;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            gameObject.TryGetComponent(out _rb);
            gameObject.TryGetComponent(out _capsule);
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
            _initialControllerHeight = _capsule.height;
            _initialControllerYCenter = _capsule.center.y;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void FixedUpdate()
        {
            if (_allowMove) { Movement(); }
            Gravity();

            _trueVelocity = (FindMove((_horizontalVelocity + Vector3.up * _verticalVelocity) / 60, 6) - transform.position) * 60;
            _speed = _trueVelocity.magnitude;
            _trueHorizontalSpeed = _trueVelocity.XZ().magnitude;
            _trueVerticalSpeed = _trueVelocity.VY().magnitude;
            transform.position += _trueVelocity / 60;
        }

        void Update()
        {
            if (_allowLook) { Rotation(); }
            Crouching();

            if (transform.position.y < 0)
            {
                //EditorApplication.isPaused = true;
            }
        }

        // Update is called once per frame

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
            _movementAngleAdjusted = _movement;


            // Moves the horizontal velocity towards the adjusted movement, decelerating any forces applied on the movement 
            _horizontalVelocity = Vector3.MoveTowards(
                _horizontalVelocity, _movementAngleAdjusted,
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
            if (Physics.SphereCast(transform.position + Vector3.up * capsule.radius, capsule.radius, Vector3.down, out RaycastHit hitDown, skinWidth * 3, config.GroundMask, QueryTriggerInteraction.Ignore) && Vector3.Angle(Vector3.up, hitDown.normal) < config.SlideAngle)
            {
                _isJumpFalling = false;
                _lastFloorNormal = hitDown.RepairHitSurfaceNormal(config.GroundMask);
                GameObject colliderObject = hitDown.collider.gameObject;
                if (!_isGrounded)
                {
                    _isGrounded = true;
                    TouchingGround.Invoke();
                }
                else
                {
                    if (colliderObject.CompareTag("MovingPlatform") && colliderObject.TryGetComponent(out MovePlayer movePlayer))
                    {
                        transform.position += movePlayer.lastMove;
                    }
                }
                _isGrounded = true;
            }
            else
            {
                _lastFloorNormal = Vector3.up;
                if (_isGrounded)
                {
                    _isGrounded = false;
                    LeavingGround.Invoke();
                }
                _isGrounded = false;
            }
            if (Physics.SphereCast(transform.position + Vector3.up * (capsule.height - capsule.radius), capsule.radius, Vector3.up, out RaycastHit hitUp, skinWidth * 2, config.GroundMask))
            {
                if (Vector3.Angle(Vector3.up, hitUp.normal) >= 135 && _verticalVelocity > 0)
                {
                    _verticalVelocity = 0;
                    _isJumping = false;
                    config.JumpHoldTimeCooldown.End();
                }
                _touchingRoof = true;
            }
            else
            {
                if (_touchingRoof)
                {
                    _touchingRoof = false;
                    if (_crouchBuffered)
                    {
                        OnCrouchReleased();
                    }
                }
                _touchingRoof = false;
            }

            if ((!isGrounded || _verticalVelocity > 0))
            {
                // Applies friction if sliding
                _verticalVelocity += -9.81f * config.GravityMultiplier * jumpMultiplier * Time.deltaTime;
                if (_verticalVelocity > 100) { _verticalVelocity = 100; }
            }
            else
            {
                _verticalVelocity = 0;
            }
        }

        public Vector3 FindMove(Vector3 moveVector, int maxBounces)
        {
            return FindMove(moveVector, maxBounces, out Vector3[] castPoints, out RaycastHit[] hits);
        }

        public Vector3 FindMove(Vector3 moveVector, int maxBounces, out Vector3[] castPoints, out RaycastHit[] hits)
        {
            castPoints = new Vector3[maxBounces + 1];
            hits = new RaycastHit[maxBounces];
            Vector3 vertical = moveVector.VY();
            Vector3 adjustedHorizontal = moveVector.XZ();
            Vector3 displacement = vertical + adjustedHorizontal;
            float radius = capsule.radius;

            Vector3 castPoint = transform.position;
            float hitAngle;

            castPoints[0] = castPoint;
            int bounce = 0;
            while (CastSelf(castPoint, displacement.normalized, displacement.magnitude + skinWidth, out RaycastHit hit) && bounce < maxBounces && displacement.magnitude > 0.002f)
            {
                hits[bounce] = hit;
                bounce++;
                Vector3 surfaceNormal = hit.RepairHitSurfaceNormal(config.GroundMask);


                hitAngle = Vector3.Angle(Vector3.up, hit.normal);
                if (hit.distance == 0)
                {
                    Debug.LogWarning("Player is inside a collider, push along normal being attempted");
                    if (verticalVelocity < 0)
                    {
                        _verticalVelocity = 0;
                    }
                    return castPoint;
                }

                Vector3 horizontalNormal = hit.normal.XZ();
                Vector3 toPoint = hit.point + horizontalNormal * radius;

                float horizontalDistanceFromWall = horizontalNormal.magnitude * radius;
                int roofOrGroundHit = hitAngle >= 89.999f ? 1 : -1;
                float verticalDistanceFromWall = Mathf.Sqrt(Mathf.Clamp(Mathf.Pow(radius, 2) - Mathf.Pow(horizontalDistanceFromWall, 2), 0, Mathf.Infinity));
                if (roofOrGroundHit > 0)
                {
                    if (hitAngle < 90.001f)
                    {
                        toPoint = toPoint.XZ() + castPoint.VY() + hit.normal * skinWidth;
                        toPoint += (adjustedHorizontal.VY() + vertical.VY()) * (toPoint.XZ() - castPoint.XZ()).magnitude / (adjustedHorizontal.XZ()).magnitude;
                    }
                    else
                    {
                        toPoint += Vector3.up * (-verticalDistanceFromWall - capsule.height + radius) + hit.normal.XZ() * skinWidth;
                    }
                }
                else
                {
                    toPoint += Vector3.up * (verticalDistanceFromWall - radius) + hit.normal * skinWidth;
                }
                bool stairSnapped = false;
                if (vertical.y == 0)
                {
                    if (roofOrGroundHit > 0)
                    {
                        if (!CastSelf(toPoint + Vector3.up * config.Stepheight + Vector3.up * skinWidth, -hit.normal.XZ(), config.MinStepDistance, out RaycastHit stairHit))
                        {
                            toPoint = toPoint.XZ() + hit.point.VY() + Vector3.up * skinWidth;
                            stairSnapped = true;
                            horizontalNormal = Vector3.zero;
                        }
                    }
                    else
                    {
                        float yOffset = hit.point.y - toPoint.y;
                        if (yOffset <= config.Stepheight)
                        {
                            if (!CastSelf(toPoint + Vector3.up * yOffset + Vector3.up * skinWidth, -hit.normal.XZ(), config.MinStepDistance, out RaycastHit stairHit))
                            {
                                toPoint = toPoint.XZ() + hit.point.VY() + Vector3.up * skinWidth;
                                stairSnapped = true;
                            }
                        }
                    }
                }


                Vector3 pointDistance = toPoint - castPoint;
                Vector3 previousDisplacementHorizontal = adjustedHorizontal;
                adjustedHorizontal -= pointDistance.XZ();

                if (adjustedHorizontal.y + vertical.y != 0)
                {
                    float adjustedYDecimal = adjustedHorizontal.y / (adjustedHorizontal.y + vertical.y);
                    adjustedHorizontal -= pointDistance.VY() * adjustedYDecimal;
                    vertical -= pointDistance.VY() * (1 - adjustedYDecimal);
                }

                if (roofOrGroundHit > 0)
                {
                    if (vertical.y > 0)
                    {
                        adjustedHorizontal = Vector3.ProjectOnPlane(adjustedHorizontal, hit.normal) + Vector3.ProjectOnPlane(vertical, hit.normal);
                        vertical = Vector3.zero;
                    }
                    else if (vertical.y == 0)
                    {
                        Vector3 normalCross = Vector3.Cross(Vector3.up, horizontalNormal.normalized);
                        float distanceMagnitude = Vector3.Dot(Vector3.Reflect(previousDisplacementHorizontal - pointDistance.XZ(), horizontalNormal.normalized), normalCross);
                        adjustedHorizontal = normalCross.XZ() * distanceMagnitude;
                    }
                    else
                    {
                        adjustedHorizontal = Vector3.ProjectOnPlane(adjustedHorizontal, hit.normal);
                    }
                }
                else
                {
                    Vector3 trueNormal = stairSnapped ? Vector3.up : hit.normal;
                    adjustedHorizontal = Vector3.ProjectOnPlane(adjustedHorizontal, trueNormal).normalized * adjustedHorizontal.magnitude;
                    if (Vector3.Angle(Vector3.up, hit.normal) > config.SlideAngle)
                    {
                        adjustedHorizontal += Vector3.ProjectOnPlane(vertical, trueNormal);
                    }
                    vertical = Vector3.zero;

                }
                displacement = vertical + adjustedHorizontal;

                if (CastSelf(castPoint, toPoint - castPoint, Vector3.Distance(toPoint, castPoint), out RaycastHit none))
                {
                    Debug.LogWarning("Reverting move attempt");
                    if (verticalVelocity < 0)
                    {
                        _verticalVelocity = 0;
                    }
                    if (roofOrGroundHit > 0)
                    {
                        return castPoint + hit.normal.XZ();
                    }
                    else
                    {
                        return castPoint + hit.normal * skinWidth;
                    }
                }

                castPoint = toPoint;
                castPoints[bounce] = castPoint;

            }

            if (bounce < maxBounces)
            {
                castPoint += displacement;

                if (verticalVelocity == 0)
                {
                    if (Vector3.Angle(Vector3.up, _lastFloorNormal) < config.SlideAngle)
                    {
                        float AngleAdjusted = _lastFloorNormal.y != 1 ? displacement.XZ().magnitude / Mathf.Tan(Vector3.Angle(Vector3.up, _lastFloorNormal) * Mathf.Deg2Rad) : 0;

                        if (CastSelf(castPoint, Vector3.down, AngleAdjusted + config.Stepheight, out RaycastHit groundLockHit) && groundLockHit.distance > skinWidth * 2)
                        {
                            //Debug.Log(groundLockHit.distance);
                            float horizontalDistanceFromWall = groundLockHit.normal.XZ().magnitude * radius;
                            float verticalDistanceFromWall = Mathf.Sqrt(Mathf.Clamp(Mathf.Pow(radius, 2) - Mathf.Pow(horizontalDistanceFromWall, 2), 0, Mathf.Infinity));
                            castPoint += (groundLockHit.point - castPoint).VY() + (groundLockHit.normal.VY() * skinWidth) - Vector3.up * (radius - verticalDistanceFromWall);
                        }
                    }
                }

                if (displacement.magnitude > 0.001f && displacement != Vector3.zero)
                {
                    castPoints[bounce + 1] = castPoint;
                }
            }
            return castPoint;
        }

        bool CastSelf(Vector3 pos, Vector3 dir, float dist, out RaycastHit hit)
        {
            // Get Parameters associated with the KCC
            Vector3 center = colliderCenter + pos;
            float radius = capsule.radius;
            float height = capsule.height;
            // Get top and bottom points of collider
            Vector3 top = center + Vector3.up * (height / 2);
            Vector3 bottom = center + Vector3.down * (height / 2);

            //Check what objects this collider will hit when cast with this configuration excluding itself
            IEnumerable<RaycastHit> hits = Physics.CapsuleCastAll(
                top - Vector3.up * radius, bottom - Vector3.down * radius, radius - Physics.defaultContactOffset, dir.normalized, dist, config.GroundMask, QueryTriggerInteraction.Ignore)
                .Where(hit => hit.collider.transform != transform);
            bool didHit = hits.Count() > 0;

            // Find the closest objects hit
            float closestDist = didHit ? Enumerable.Min(hits.Select(hit => hit.distance)) : 0;
            IEnumerable<RaycastHit> closestHit = hits.Where(hit => hit.distance == closestDist);

            // Get the first hit object out of the things the player collides with
            hit = closestHit.FirstOrDefault();

            // Return if any objects were hit
            return didHit;
            //return Physics.CapsuleCast(top - Vector3.up * radius, bottom + Vector3.up * radius, radius, dir, out hit, dist, config.GroundMask, QueryTriggerInteraction.Ignore);
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
            if (!_allowJump || _touchingRoof) { return; }
            _isJumping = true;
            //OnCrouchReleased();
            config.CoyoteTimeCooldown.End();
            config.JumpCooldown.Start();

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
            capsule.height = _initialControllerHeight + _crouchOffset;
            _playerCamera.transform.localPosition = new Vector3(_playerCamera.transform.localPosition.x, _initialCameraY + _crouchOffset, _playerCamera.transform.localPosition.z);
            capsule.center = colliderCenter.XZ() + Vector3.up * (_initialControllerYCenter + _crouchOffset / 2);
            if (_hurtbox != null)
            {
                _hurtbox.height = _initialControllerHeight + _crouchOffset;
                _hurtbox.center = colliderCenter.XZ() + Vector3.up * (_initialControllerYCenter + _crouchOffset / 2);
            }
        }


    }

#if UNITY_EDITOR
    public static class PlayerControllerRBGizmos
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        public static void DrawGizmoForPlayerControllerRB(KineticPlayerController source, GizmoType gizmoType)
        {
            CapsuleCollider capsule = source.capsule;
            float radius = capsule.radius;
            float height = capsule.height;
            Quaternion rotation = source.transform.rotation;

            Gizmos.DrawMesh(CreatMesh.CapsuleData(32, 16, 0, 0, radius), source.transform.position + Vector3.up * (capsule.radius + capsule.radius), rotation);

            if (!source.enableMovePreview) { return; }


            Vector3 moveVector = (source.movement + Vector3.up * source.verticalVelocity) == Vector3.zero ? source.transform.TransformDirection(source.castDisplacement) : (source.movement + Vector3.up * source.verticalVelocity);
            source.FindMove(moveVector, source.maxDebugBounces, out Vector3[] castPoints, out RaycastHit[] hitPoints);
            source.currentDebugBounces = 0;
            for (int i = 1; i < castPoints.Length; i++)
            {
                if (castPoints[i] == Vector3.zero) { return; }
                source.currentDebugBounces++;
                Color colliderColour = Gizmos.color = new Color(0, 1, 0, 0.5f);
                if (hitPoints[i - 1].point != Vector3.zero)
                {
                    Gizmos.DrawIcon(hitPoints[i - 1].point, "Hit", source.showHitPoints, Color.HSVToRGB((i - 1) * 10 / 360.0f, 1.0f, 1.0f));

                    Gizmos.color = new Color(0, 0, 1, 1f);
                    Gizmos.DrawLine(castPoints[i], castPoints[i] + hitPoints[i - 1].normal);

                    colliderColour = new Color(1, 1, 0, 0.5f);
                }
                Gizmos.color = new Color(1, 0, 0, 1f);
                Gizmos.DrawLine(castPoints[i - 1], castPoints[i]);

                Gizmos.color = colliderColour;
                Gizmos.DrawMesh(CreatMesh.CapsuleData(32, 16, 0, height / 2, radius), castPoints[i] + Vector3.up * height / 2, rotation);
            }
        }
    }
#endif
}