namespace Inherited {
    using UnityEngine;

    public abstract class Damageable {
        protected string Name { get; private set; }
        private int _currentHealthPoints;
        protected readonly int MaxHealthPoints;

        protected Damageable(string name, int maxHealthPoints=100) {
            MaxHealthPoints = maxHealthPoints;
            _currentHealthPoints = MaxHealthPoints;
            Name = name;
        }
        
        public int HeathPoints {
            get => _currentHealthPoints;
            set {
                if (value >= _currentHealthPoints) {
                    Debug.Log("Incorrect damage value!");
                }
                else if (value <= 0) {
                    _currentHealthPoints = 0;
                    Debug.Log("Damage exceeds current hp value, so set to 0");
                }
                else {
                    _currentHealthPoints = value;
                }
            }
        }

        public abstract void TakeDamage(int damageValue);

        public virtual void ShowStat() {
            Debug.Log($"{Name} with {_currentHealthPoints}/{MaxHealthPoints}HP");
        }
    }
}