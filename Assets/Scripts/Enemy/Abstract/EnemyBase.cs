using Enemy.Damageable;
using UnityEngine;

namespace Enemy.Abstract {
    public abstract class EnemyBase : MonoBehaviour, IDamageable {
        [SerializeField] protected float maxHealth;
        protected float CurrentHealth;
        [SerializeField] protected float damage;

        protected void Awake() {
            CurrentHealth = maxHealth;
        }

        public abstract void Damage(float value);
    }
}