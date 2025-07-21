using DG.Tweening;
using Enemy.Abstract;
using UnityEngine;

namespace Enemy.Definitions {
    public class ImmortalEnemy : EnemyBase {
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private Color damagedColor;
        [SerializeField] private float damagedColorFadeOut;
        private Tweener _damagedAnimation;
        
        public override void Damage(float value) {
            if (_damagedAnimation is { active: true }) _damagedAnimation.Kill();
            
            var startColor = enemyMaterial.color;
            enemyMaterial.color = damagedColor;
            _damagedAnimation = enemyMaterial.DOColor(startColor, damagedColorFadeOut);
        }
    }
}