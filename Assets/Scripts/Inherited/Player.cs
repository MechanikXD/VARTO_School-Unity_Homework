using UnityEngine;

namespace Inherited {
    public class Player : Damageable {
        public int PlayerExperience { get; private set; }
        
        public Player(string name, int maxHealthPoints = 100, int playerExperience = 0) 
                : base(name, maxHealthPoints) {
            PlayerExperience = playerExperience;
        }

        public override void TakeDamage(int damageValue) {
            HeathPoints -= damageValue; // It's save since all the check are made in Damageable
            Debug.Log($"My name is {Name}: after hitting with force: {damageValue} I have health: {HeathPoints}");
        }

        public override void ShowStat() {
            Debug.Log($"{Name} with {HeathPoints}/{MaxHealthPoints} HP with {PlayerExperience} XP");
        }
    }
}