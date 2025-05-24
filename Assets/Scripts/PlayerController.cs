using Inherited;
using UnityEngine;
using UnityEngine.InputSystem;

// Global namespace

public class PlayerController : MonoBehaviour {
    private Vector3 _playerMoveDirection;
    private Player _player;
    [SerializeField] private string playerName;
    [SerializeField] private int playerMaxHealth;
    [SerializeField] private int playerDamage;
    
    [SerializeField] private Rigidbody playerBody;
    [SerializeField] private float moveSpeed;

    void Start() => _player = new Player(playerName, playerMaxHealth);

    void Update() {
        var newPosition = playerBody.position + _playerMoveDirection.normalized * moveSpeed;
        playerBody.position = newPosition;
    }

    private void OnCollisionEnter(Collision other) {
        // ReSharper disable once Unity.UnknownTag idk how to tell compiler that this tag exists, soooooo...
        if (!other.gameObject.TryGetComponent<DragonController>(out var otherDragon)) {
            return;
        }

        _player.TakeDamage(otherDragon.Dragon.OnHitDamageValue);
        otherDragon.Dragon.TakeDamage(playerDamage);
    }

    public void OnMove(InputValue moveDirection) {
        var moveVector = moveDirection.Get<Vector2>();
        _playerMoveDirection = new Vector3(moveVector.x, 0, moveVector.y);
    }
}