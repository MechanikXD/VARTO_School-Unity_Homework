using System.Collections;
using Enemy.Damageable;
using UnityEngine;
using Weapons.Ammunition;

namespace Weapons {
    public class WeaponBase : MonoBehaviour {
        [SerializeField] private WeaponSettings setting;
        // TODO: Better move this into settings class
        [SerializeField] private Transform shootOrigin;
        [SerializeField] private Bullet bulletPrefab;
        private int _currentAmmoCount;

        private bool _inFireDelay;

        private void Awake() => _currentAmmoCount = setting.MaxAmmo;

        public void Shoot() {
            if (_inFireDelay || _currentAmmoCount <= 0) return;

            var forwardRay =
                Camera.main!.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            
            if (Physics.Raycast(forwardRay, out var hit, setting.MaxDistance)) {
                if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable))
                    damageable.Damage(setting.GetWeaponDamage(hit.distance));
            }
            
            var bulletDirection = forwardRay.direction.normalized;
            var bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.LookRotation(bulletDirection));
            bullet.AddForce(bulletDirection, setting.BulletSpeed);

            _inFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(setting.FireDelay));
            _currentAmmoCount--;
        }

        public void Reload() {
            _inFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(setting.ReloadTime));
        }

        private IEnumerator RemoveFireDelayLater(float time) {
            yield return new WaitForSeconds(time);
            _inFireDelay = false;
        }
    }
}