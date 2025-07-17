using System;
using UnityEngine;
using Weapons.Ammunition;

namespace Weapons.Abstract {
    [Serializable, CreateAssetMenu(fileName = "WeaponSettings", menuName = "Scriptable Objects/WeaponSettings")]
    public class WeaponSettings : ScriptableObject {
        [SerializeField] private float fireDelay;
        [SerializeField] private float reloadTime;
        [SerializeField] private bool isAutomatic;
        [Space]
        [SerializeField] private float damage;
        [SerializeField] private DamageFallOffType damageFallOff;
        [SerializeField] private float maxDistance;
        [Space]
        [SerializeField] private int maxAmmo;
        [SerializeField] private float bulletSpeed;

        public bool IsAutomatic => isAutomatic;
        public float FireDelay => fireDelay;
        public int MaxAmmo => maxAmmo;
        public float MaxDistance => maxDistance;
        public float BulletSpeed => bulletSpeed;
        public float ReloadTime => reloadTime;

        public float GetWeaponDamage(float distance) {
            var clampDistance = Mathf.Clamp(distance, 0, maxDistance - 1);
            return damageFallOff switch {
                DamageFallOffType.None => damage,
                DamageFallOffType.Linear => -damage / maxDistance * clampDistance + damage,
                DamageFallOffType.Exponential => -Mathf.Exp(clampDistance - maxDistance) * damage +
                                                 damage,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}