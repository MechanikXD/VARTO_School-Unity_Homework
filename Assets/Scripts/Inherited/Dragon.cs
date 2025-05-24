using UnityEngine;

namespace Inherited {
    public class Dragon : Damageable {
        private Damageable _dragon;

        public int OnHitDamageValue { get; private set; }

        public Dragon(string name, int onHitDamageValue, int maxHealthPoints = 100) 
                : base(name, maxHealthPoints) {
            OnHitDamageValue = onHitDamageValue;
        }
        
        public override void TakeDamage(int damageValue) {
            _dragon.HeathPoints -= damageValue;
            Debug.Log($"I, the mighty dragon, have lost: {damageValue} hit points from a hunter's shot!");
        }
    }
}