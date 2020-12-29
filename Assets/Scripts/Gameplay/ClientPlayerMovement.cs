using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class ClientPlayerMovement : NetworkBehaviour
    {
        [SerializeField] private Transform playerCam;
        [SerializeField] private Transform orientation;

        [Header("Inputs")]
        [SerializeField] private Vector2 moveInputs; // X for move left/right, Y for move forward/back
        [SerializeField] private Vector2 lookInputs; // X for rotate left/right, Y for look up/down

        private Rigidbody _playerRigidbody;
        private Animator _animator;

        [Header("Rotation & look")]
        [SerializeField] private float lookAngleRange = 60f; // 60' up, 60' down
        [SerializeField] private float lookSensitivity = 50f;
        private float _lookSensitivityMultiplier = 1f;
        private float _lookRotationX;
        private float _desiredX;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 4500;
        [SerializeField] private float maxSpeed = 20;
        [SerializeField] private bool grounded;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float counterMovement = 0.175f;
        [SerializeField] private float maxSlopeAngle = 35f;
        private float _moveThreshold = 0.01f;

        [Header("Crouching")]
        [SerializeField] private float slideCounterMovement = 0.2f;
        private Vector3 _normalVector = Vector3.up;

        [Header("Jumping")]
        [SerializeField] private float jumpForce = 550f;
        private bool _readyToJump = true;
        private float _jumpCooldown = 0.25f;

        //Inputs
        private bool _jumping, _crouching;
        private bool _crouchToggleRequest;

        #region Receive Input Values
        // Call these functions from the PlayerInput component as set up by this guide:
        // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/QuickStartGuide.html#getting-input-indirectly-through-an-input-action
        // This is designed for you to use "Action Responses" when 
        // Player Input Behaviour is set to "Invoke Unity Events", 
        // and then you can point those events to these functions.

        // Because the CallbackContext object is generic, you must call ReadValue<T>
        // and specify the type that you're expecting. Read more on C# generics here:
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/

        public void UpdateMoveInputs(InputAction.CallbackContext context) 
            => moveInputs = context.ReadValue<Vector2>();

        public void UpdateLookInputs(InputAction.CallbackContext context) 
            => lookInputs = context.ReadValue<Vector2>();

        public void UpdateJumpInput(InputAction.CallbackContext context)
            => _jumping = context.ReadValueAsButton();

        public void UpdateCrouchInput(InputAction.CallbackContext context)
            => _crouchToggleRequest = context.action.triggered && context.ReadValue<float>() > 0;

        #endregion

        public override void OnStartLocalPlayer()
        {
            if (!isServer)
                enabled = true;
        }

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate() 
            => Move();

        private void Update()
            => Look();

        private void Move()
        {
            if (!hasAuthority)
                return;

            //Extra gravity
            _playerRigidbody.AddForce(Vector3.down * (Time.deltaTime * 10));

            //Find actual velocity relative to where player is looking
            Vector2 mag = FindVelRelativeToLook();
            float xMag = mag.x, yMag = mag.y;

            //Counteract sliding and sloppy movement
            CounterMovement(moveInputs.x, moveInputs.y, mag);

            //If holding jump && ready to jump, then jump
            if (_readyToJump && _jumping && !_crouching) 
                Jump();

            if (_crouchToggleRequest)
                HandleCrouching();

            //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
            if (moveInputs.x > 0 && xMag > maxSpeed) moveInputs.x = 0;
            if (moveInputs.x < 0 && xMag < -maxSpeed) moveInputs.x = 0;
            if (moveInputs.y > 0 && yMag > maxSpeed) moveInputs.y = 0;
            if (moveInputs.y < 0 && yMag < -maxSpeed) moveInputs.y = 0;

            //Some multipliers
            float multiplier = 1f, multiplierV = 1f;

            // Move in air
            if (!grounded)
            {
                multiplier = 0.5f;
                multiplierV = 0.5f;
            }

            // Move while crouching
            if (grounded && _crouching)
                multiplierV = 0.8f;

            //Apply forces to move player
            _playerRigidbody.AddForce(orientation.transform.forward * (moveInputs.y * moveSpeed * Time.deltaTime * multiplier * multiplierV));
            _playerRigidbody.AddForce(orientation.transform.right * (moveInputs.x * moveSpeed * Time.deltaTime * multiplier));

            _animator.SetFloat("Horizontal", moveInputs.x, 0.1f, Time.fixedDeltaTime);
            _animator.SetFloat("Vertical", moveInputs.y, 0.1f, Time.fixedDeltaTime);
        }

        private void Jump()
        {
            if (grounded && _readyToJump)
            {
                _readyToJump = false;
                _animator.SetTrigger("Jump");
                _animator.ResetTrigger("Jump");

                //Add jump forces
                _playerRigidbody.AddForce(Vector2.up * (jumpForce * 1.5f));
                _playerRigidbody.AddForce(_normalVector * (jumpForce * 0.5f));

                //If jumping while falling, reset y velocity.
                var velocity = _playerRigidbody.velocity;
                if (_playerRigidbody.velocity.y < 0.5f)
                    _playerRigidbody.velocity = new Vector3(velocity.x, 0, velocity.z);
                else if (_playerRigidbody.velocity.y > 0)
                    _playerRigidbody.velocity = new Vector3(velocity.x, velocity.y / 2, velocity.z);

                Invoke(nameof(ResetJump), _jumpCooldown);
            }
        }

        private void ResetJump() 
            => _readyToJump = true;

        private void HandleCrouching()
        {
            _crouching = !_crouching;
            _crouchToggleRequest = false;

            if (_crouching)
                StartCrouch();
            else
                StopCrouch();
        }

        private void StartCrouch() 
            => _animator.SetFloat("Crouch", 1f);

        private void StopCrouch() 
            => _animator.SetFloat("Crouch", 0f);

        private void Look()
        {
            if (!hasAuthority)
                return;

            var mouseLook = lookInputs * (lookSensitivity * Time.fixedDeltaTime * _lookSensitivityMultiplier);

            //Find current look rotation
            var currentRotation = playerCam.transform.localRotation.eulerAngles;
            _desiredX = currentRotation.y + mouseLook.x;

            //Rotate, and also make sure we don't over-rotate or under-rotate.
            _lookRotationX -= mouseLook.y;
            _lookRotationX = Mathf.Clamp(_lookRotationX, -lookAngleRange, lookAngleRange);

            //Perform the rotations
            playerCam.transform.localRotation = Quaternion.Euler(_lookRotationX, _desiredX, 0);
            orientation.transform.localRotation = Quaternion.Euler(0, _desiredX, 0);
        }

        private void CounterMovement(float x, float y, Vector2 mag)
        {
            if (!grounded || _jumping)
                return;

            //Slow down sliding
            if (_crouching)
            {
                _playerRigidbody.AddForce(moveSpeed * Time.deltaTime * -_playerRigidbody.velocity.normalized * slideCounterMovement);
                return;
            }

            //Counter movement
            if (Math.Abs(mag.x) > _moveThreshold && Math.Abs(x) < 0.05f || (mag.x < -_moveThreshold && x > 0) || (mag.x > _moveThreshold && x < 0))
            {
                _playerRigidbody.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
            }
            if (Math.Abs(mag.y) > _moveThreshold && Math.Abs(y) < 0.05f || (mag.y < -_moveThreshold && y > 0) || (mag.y > _moveThreshold && y < 0))
            {
                _playerRigidbody.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
            }

            //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
            if (Mathf.Sqrt((Mathf.Pow(_playerRigidbody.velocity.x, 2) + Mathf.Pow(_playerRigidbody.velocity.z, 2))) > maxSpeed)
            {
                float fallspeed = _playerRigidbody.velocity.y;
                Vector3 n = _playerRigidbody.velocity.normalized * maxSpeed;
                _playerRigidbody.velocity = new Vector3(n.x, fallspeed, n.z);
            }
        }

        /// <summary>
        /// Find the velocity relative to where the player is looking
        /// Useful for vectors calculations regarding movement and limiting movement
        /// </summary>
        /// <returns></returns>
        public Vector2 FindVelRelativeToLook()
        {
            float lookAngle = orientation.transform.eulerAngles.y;
            float moveAngle = Mathf.Atan2(_playerRigidbody.velocity.x, _playerRigidbody.velocity.z) * Mathf.Rad2Deg;

            float u = Mathf.DeltaAngle(lookAngle, moveAngle);
            float v = 90 - u;

            float magnitue = _playerRigidbody.velocity.magnitude;
            float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
            float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

            return new Vector2(xMag, yMag);
        }

        private bool IsFloor(Vector3 v)
        {
            float angle = Vector3.Angle(Vector3.up, v);
            return angle < maxSlopeAngle;
        }

        private bool _cancellingGrounded;

        /// <summary>
        /// Handle ground detection
        /// </summary>
        private void OnCollisionStay(Collision other)
        {
            //Make sure we are only checking for walkable layers
            int layer = other.gameObject.layer;
            if (groundLayer != (groundLayer | (1 << layer))) return;

            //Iterate through every collision in a physics update
            for (int i = 0; i < other.contactCount; i++)
            {
                Vector3 normal = other.contacts[i].normal;
                //FLOOR
                if (IsFloor(normal))
                {
                    grounded = true;
                    _cancellingGrounded = false;
                    _normalVector = normal;
                    CancelInvoke(nameof(StopGrounded));
                }
            }

            //Invoke ground/wall cancel, since we can't check normals with CollisionExit
            float delay = 3f;
            if (!_cancellingGrounded)
            {
                _cancellingGrounded = true;
                Invoke(nameof(StopGrounded), Time.deltaTime * delay);
            }
        }

        private void StopGrounded() 
            => grounded = false;
    }
}