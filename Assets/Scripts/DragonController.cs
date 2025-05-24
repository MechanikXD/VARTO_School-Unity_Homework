using Inherited;
using UnityEngine;

// Global namespace

public class DragonController : MonoBehaviour {
    // Player has all tha logic, so this class here only to initialize the dragon class
    private Damageable _dragon;
    [SerializeField] private string dragonName;
    [SerializeField] private int dragonMaxHealth;

    void Start() => _dragon = new Dragon(dragonName, dragonMaxHealth);
}