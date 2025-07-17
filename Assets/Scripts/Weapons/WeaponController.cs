using System;
using UnityEngine;
using Weapons.Abstract;

namespace Weapons {
    [Serializable]
    public class WeaponController {
        [SerializeField] private WeaponBase[] weaponArray;
        private int _currentWeaponIndex;

        public WeaponBase CurrentWeapon => weaponArray[_currentWeaponIndex];

        public void InitializeSelf() {
            foreach (var weapon in weaponArray) weapon.gameObject.SetActive(false);
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangeNextWeapon() {
            weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex++;
            if (_currentWeaponIndex == weaponArray.Length) _currentWeaponIndex = 0;
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangePreviousWeapon() {
            weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex--;
            if (_currentWeaponIndex < 0) _currentWeaponIndex = weaponArray.Length - 1;
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void Shoot(bool isShootings) => weaponArray[_currentWeaponIndex].Shoot(isShootings);
    }
}