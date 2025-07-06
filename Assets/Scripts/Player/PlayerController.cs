using UnityEngine;
using UnityEngine.InputSystem;

namespace Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private CharacterController controller;
        [SerializeField] private Transform attachedCamera;
        [SerializeField] private float gravity;
        
        [SerializeField] private float mouseSensitivity;
        private Vector2 _lookDirection; // Store data from OnLook method
        private float _cameraRotation; // Camera rotation along X axis only
        private float _bodyRotation; // Rotation of player along Y axis

        private Vector2 _moveDirection;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float jumpSpeed;

        private void Start() {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() {
            controller.Move(GetCameraRelativeVector() * Time.deltaTime);

            // Get mouse position
            var lookDirection = _lookDirection * (mouseSensitivity * Time.deltaTime);
            // Rotate player instead of camera
            _bodyRotation += lookDirection.x;
            transform.rotation = Quaternion.Euler(0f, _bodyRotation, 0f);
            // Camera rotate here
            _cameraRotation -= lookDirection.y;
            _cameraRotation = Mathf.Clamp(_cameraRotation, -70f, 70f);
            attachedCamera.localRotation = Quaternion.Euler(_cameraRotation, 0f, 0f);
        }

        private Vector3 GetCameraRelativeVector() {
            Vector3 playerRelativeVector = (_moveDirection.y * attachedCamera.forward +
                                            _moveDirection.x * attachedCamera.right) * moveSpeed;
            playerRelativeVector.y = -gravity;
            return playerRelativeVector;
        }
        
        public void OnMove(InputValue currentMoveDirection) => _moveDirection = currentMoveDirection.Get<Vector2>();

        public void OnLook(InputValue lookDirection) => _lookDirection = lookDirection.Get<Vector2>();

        public void OnJump() {
            if (controller.isGrounded) controller.Move(attachedCamera.up.normalized * jumpSpeed);
        }
    }
}