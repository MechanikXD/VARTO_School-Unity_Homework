using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform attachedCamera;
        [SerializeField] private WeaponBase playerWeapon;
        
        [SerializeField] private float mouseSensitivity;
        private float _cameraRotation; // Camera rotation along X axis only
        private float _bodyRotation; // Rotation of player along Y axis

        private Vector2 _moveDirection;
        [SerializeField] private float moveSpeed;
        
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float gravity;
        [SerializeField] private ParticleSystem landParticles;

        private float _currentJumpDuration;
        private bool _isJumping;
        private bool _lastFrameWasGrounded;

        private void Start() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _lastFrameWasGrounded = true;
        }

        private void Update() {
            controller.Move(GetCameraRelativeVector() * Time.deltaTime);
            
            if (!_lastFrameWasGrounded && controller.isGrounded) {
                landParticles.Play();
            }

            _lastFrameWasGrounded = controller.isGrounded;
        }

        private Vector3 GetCameraRelativeVector() {
            Vector3 playerRelativeVector = (_moveDirection.y * attachedCamera.forward +
                                            _moveDirection.x * attachedCamera.right) * moveSpeed;
            if (_isJumping) {
                playerRelativeVector.y = jumpSpeed;
                _currentJumpDuration += Time.deltaTime;
                
                if (_currentJumpDuration >= jumpDuration) {
                    _currentJumpDuration = 0f;
                    _isJumping = false;
                }
            }
            else {
                playerRelativeVector.y = -gravity;
            }
            return playerRelativeVector;
        }
        
        public void OnMove(InputValue currentMoveDirection) =>
            _moveDirection = currentMoveDirection.Get<Vector2>();

        public void OnLook(InputValue lookDirection) {
            // Get mouse position
            var look = lookDirection.Get<Vector2>() * (mouseSensitivity * Time.deltaTime);
            // Rotate player instead of camera
            _bodyRotation += look.x;
            transform.rotation = Quaternion.Euler(0f, _bodyRotation, 0f);
            // Camera rotate here
            _cameraRotation -= look.y;
            _cameraRotation = Mathf.Clamp(_cameraRotation, -70f, 70f);
            attachedCamera.localRotation = Quaternion.Euler(_cameraRotation, 0f, 0f);
        }

        public void OnJump() {
            if (controller.isGrounded) _isJumping = true;
        }

        public void OnAttack() => playerWeapon.Shoot();
    }
}