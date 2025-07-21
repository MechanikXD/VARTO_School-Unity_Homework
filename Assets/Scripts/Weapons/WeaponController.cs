using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons.Abstract;

namespace Weapons {
    [Serializable]
    public class WeaponController {
        [SerializeField] private Transform weaponAttachPoint;
        [SerializeField] private float throwForce = 5f;
        [SerializeField] private List<WeaponBase> weaponArray;
        private int _currentWeaponIndex;

        public WeaponBase CurrentWeapon =>
            weaponArray.Count > 0 ? weaponArray[_currentWeaponIndex] : null;

        public void InitializeSelf() {
            foreach (var weapon in weaponArray) weapon.gameObject.SetActive(false);
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void DropCurrentWeapon() {
            if (weaponArray.Count == 0) return;
            // Remove current weapon from the list
            var currentWeapon = weaponArray[_currentWeaponIndex];
            weaponArray.Remove(weaponArray[_currentWeaponIndex]);
            // Select previous if possible 
            if (weaponArray.Count > 0) {
                _currentWeaponIndex--;
                if (_currentWeaponIndex < 0) _currentWeaponIndex = weaponArray.Count - 1;
                weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
            }
            // Detach and send flying forward
            currentWeapon.DetachFromParent();
            currentWeapon.ThrowForward(throwForce);
        }

        public bool ContainsWeapon(WeaponBase weapon) => weaponArray.Contains(weapon);

        public void AddWeapon(WeaponBase newWeapon) {
            // Append new weapon
            newWeapon.AttachTo(weaponAttachPoint);
            weaponArray.Add(newWeapon);
            // Select new weapon as active (it's always at the very end of list)
            weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex = weaponArray.Count - 1;
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangeNextWeapon() {
            if (weaponArray.Count == 0) return;
            
            weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex++;
            if (_currentWeaponIndex == weaponArray.Count) _currentWeaponIndex = 0;
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangePreviousWeapon() {
            if (weaponArray.Count == 0) return;
            
            weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex--;
            if (_currentWeaponIndex < 0) _currentWeaponIndex = weaponArray.Count - 1;
            weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void Shoot(bool isShootings) {
            if (weaponArray.Count == 0) return;
            weaponArray[_currentWeaponIndex].Shoot(isShootings);
        }
    }
}