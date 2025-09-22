using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons.Abstract;

namespace Weapons {
    [Serializable]
    public class WeaponController {
        [SerializeField] private Transform _weaponAttachPoint;
        [SerializeField] private float _throwForce = 5f;
        [SerializeField] private List<WeaponBase> _weaponArray;
        private int _currentWeaponIndex;

        public WeaponBase CurrentWeapon =>
            _weaponArray.Count > 0 ? _weaponArray[_currentWeaponIndex] : null;

        public void InitializeSelf() {
            foreach (var weapon in _weaponArray) weapon.gameObject.SetActive(false);
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void DropCurrentWeapon() {
            if (_weaponArray.Count == 0) return;
            // Remove current weapon from the list
            var currentWeapon = _weaponArray[_currentWeaponIndex];
            _weaponArray.Remove(_weaponArray[_currentWeaponIndex]);
            // Select previous if possible 
            if (_weaponArray.Count > 0) {
                _currentWeaponIndex--;
                if (_currentWeaponIndex < 0) _currentWeaponIndex = _weaponArray.Count - 1;
                _weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
            }
            // Detach and send flying forward
            currentWeapon.DetachFromParent();
            currentWeapon.ThrowForward(_throwForce);
        }

        public bool ContainsWeapon(Abstract.WeaponBase weapon) => _weaponArray.Contains(weapon);

        public void AddWeapon(Abstract.WeaponBase newWeapon) {
            // Append new weapon
            newWeapon.AttachTo(_weaponAttachPoint);
            _weaponArray.Add(newWeapon);
            // Select new weapon as active (it's always at the very end of list)
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex = _weaponArray.Count - 1;
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangeNextWeapon() {
            if (_weaponArray.Count == 0) return;
            
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex++;
            if (_currentWeaponIndex == _weaponArray.Count) _currentWeaponIndex = 0;
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void ChangePreviousWeapon() {
            if (_weaponArray.Count == 0) return;
            
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(false);
            _currentWeaponIndex--;
            if (_currentWeaponIndex < 0) _currentWeaponIndex = _weaponArray.Count - 1;
            _weaponArray[_currentWeaponIndex].gameObject.SetActive(true);
        }

        public void Shoot(bool isShootings) {
            if (_weaponArray.Count == 0) return;
            _weaponArray[_currentWeaponIndex].Shoot(isShootings);
        }
    }
}