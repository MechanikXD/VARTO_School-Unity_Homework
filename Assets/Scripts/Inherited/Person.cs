namespace Inherited {
    using UnityEngine;

    public class Person {
        public string Name { get; private set; }
        private int _currentHealthPoints;
        public readonly int MaxHealthPoints;

        public Person(string name, int maxHealthPoints=100) {
            MaxHealthPoints = maxHealthPoints;
            _currentHealthPoints = MaxHealthPoints;
            Name = name;
        }
        
        public int HeathPoints {
            get => _currentHealthPoints;
            set {
                if (value <= 0) {
                    Debug.Log("Incorrect damage value!");
                }
                else {
                    var resultHealthPoints = _currentHealthPoints - value;
                    if (resultHealthPoints <= 0) {
                        _currentHealthPoints = 0;
                        Debug.Log("Damage exceeds current hp value, so set to 0");
                    }
                    else {
                        _currentHealthPoints = resultHealthPoints;
                    }
                }
            }
        }

        public virtual void ShowStat() {
            Debug.Log($"{Name} with {_currentHealthPoints}/{MaxHealthPoints}HP");
        }
    }
}