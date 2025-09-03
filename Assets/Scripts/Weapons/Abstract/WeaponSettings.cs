using System;
using UnityEngine;
using Weapons.Ammunition;

namespace Weapons.Abstract {
    [Serializable, CreateAssetMenu(fileName = "WeaponSettings", menuName = "Scriptable Objects/WeaponSettings")]
    public class WeaponSettings : ScriptableObject {
        [SerializeField] private ShootType _shootType;
        [SerializeField] private float _fireDelay;
        [SerializeField] private float _reloadTime;
        [SerializeField] private bool _isAutomatic;
        [Space]
        [SerializeField] private float _damage;
        [SerializeField] private DamageFallOffType _damageFallOff;
        [SerializeField] private float _maxDistance;
        [Space]
        [SerializeField] private int _maxAmmo;
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private AudioClip _shootSound;

        [Space]
        [SerializeField] private float _deviationAngle;
        [SerializeField] private float _queueLength;
        [SerializeField] private float _burstPallets;

        public ShootType Type => _shootType;
        public bool IsAutomatic => _isAutomatic;
        public float FireDelay => _fireDelay;
        public int MaxAmmo => _maxAmmo;
        public float MaxDistance => _maxDistance;
        public float BulletSpeed => _bulletSpeed;
        public float ReloadTime => _reloadTime;
        public AudioClip ShootSound => _shootSound;
        public float DeviationAngle => _deviationAngle;
        public float QueueLength => _queueLength;
        public float BurstPalletCount => _burstPallets;

        public float GetWeaponDamage(float distance) {
            var clampDistance = Mathf.Clamp(distance, 0, _maxDistance - 1);
            return _damageFallOff switch {
                DamageFallOffType.None => _damage,
                DamageFallOffType.Linear => -_damage / _maxDistance * clampDistance + _damage,
                DamageFallOffType.Exponential => -Mathf.Exp(clampDistance - _maxDistance) * _damage +
                                                 _damage,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public enum ShootType{
        Straight,
        WithDeviation,
        StraightQueue,
        QueueWithDeviation,
        Burst,
        Custom
    }
}