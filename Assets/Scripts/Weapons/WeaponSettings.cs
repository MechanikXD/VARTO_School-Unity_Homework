using System;
using UnityEngine;
using Weapons.Ammunition;

namespace Weapons {
    [Serializable, CreateAssetMenu(fileName = "WeaponSettings", menuName = "Scriptable Objects/WeaponSettings")]
    // TODO: Add shoot variants e.g: forward, random in range, spread.
    public class WeaponSettings : ScriptableObject {
        [SerializeField] private float fireDelay;
        [SerializeField] private float reloadTime;
        [Space]
        [SerializeField] private float damage;
        [SerializeField] private DamageFallOffType damageFallOff;
        [SerializeField] private float maxDistance;
        [Space]
        [SerializeField] private int maxAmmo;
        [SerializeField] private float bulletSpeed;


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