using UnityEngine;

namespace Inherited {
    public class Player : Person {
        public int PlayerExperience { get; private set; }
        
        public Player(string name, int maxHealthPoints=100, int playerExperience=0) : base(name, maxHealthPoints) {
            PlayerExperience = playerExperience;
        }

        public override void ShowStat() {
            Debug.Log($"{Name} with {HeathPoints}/{MaxHealthPoints} HP with {PlayerExperience} XP");
        }
    }
}