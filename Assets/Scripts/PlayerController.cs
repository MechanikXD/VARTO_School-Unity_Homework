using UnityEngine;
using UnityEngine.InputSystem;

// Global namespace

public class PlayerController : MonoBehaviour {
    private Vector3 _playerMoveDirection;
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float moveSpeed;

    void Update() {
        var newPosition = playerBody.position + _playerMoveDirection.normalized * moveSpeed;
        newPosition.z = 0;  // Fix on z axis, so it won't change on collision
        playerBody.position = newPosition;
    }

    private void OnCollisionEnter(Collision other) {
        // ReSharper disable once Unity.UnknownTag idk how to tell compiler that this tag exists, soooooo...
        if (other.gameObject.CompareTag("Enemy")) {
            Debug.Log("PLayer collided with enemy! ewwww...");
        }
    }

    public void OnMove(InputValue moveDirection) {
        var moveVector = moveDirection.Get<Vector2>();
        _playerMoveDirection = new Vector3(-moveVector.x, 0, moveVector.y);
    }
}