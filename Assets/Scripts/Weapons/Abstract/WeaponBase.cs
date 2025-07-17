using System;
using System.Collections;
using Enemy.Damageable;
using UnityEngine;
using Weapons.Ammunition;
using Random = UnityEngine.Random;

namespace Weapons.Abstract {
    public abstract class WeaponBase : MonoBehaviour {
        [SerializeField] protected WeaponSettings setting;
        [SerializeField] protected Transform shootOrigin;
        [SerializeField] protected Bullet bulletPrefab;
        protected bool InFireDelay;
        protected int CurrentAmmoCount;

        private Func<Vector3, Ray> _screenPointToRay;

        private bool _isShooting;
        
        private void Awake() {
            CurrentAmmoCount = setting.MaxAmmo;
            _screenPointToRay = Camera.main!.ScreenPointToRay;
        }

        private void SetContinuousShooting(bool isContinuous) {
            if (isContinuous && !_isShooting) {
                _isShooting = true;

                IEnumerator ShootContinuously() {
                    while (_isShooting) {
                        ShootAction();
                        yield return new WaitForSeconds(setting.FireDelay);
                    }
                }

                StartCoroutine(ShootContinuously());
            }
            else if (!isContinuous && _isShooting) {
                _isShooting = false;
            }
        }

        protected abstract void ShootAction();

        public void Shoot(bool isShooting) {
            if (setting.IsAutomatic) {
                SetContinuousShooting(isShooting);
            }
            else {
                ShootAction();
            }
        }

        protected void ShootInDirection(Ray direction) {
            if (InFireDelay || CurrentAmmoCount <= 0) return;

            if (Physics.Raycast(direction, out var hit, setting.MaxDistance)) {
                if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable))
                    damageable.Damage(setting.GetWeaponDamage(hit.distance));
            }
            
            var bulletDirection = direction.direction.normalized;
            var bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.LookRotation(bulletDirection));
            bullet.AddForce(bulletDirection, setting.BulletSpeed);

            InFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(setting.FireDelay));
            CurrentAmmoCount--;
        }

        protected void ShootForward() {
            var forwardRay = _screenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

            ShootInDirection(forwardRay);
        }

        protected void ShootForwardWithDeviation(float angleDeviation) {
            var widthDeviation = Screen.width / 2f * (1f - angleDeviation / 90f);
            var heightDeviation = Screen.height / 2f * (1f - angleDeviation / 90f);

            var ray = _screenPointToRay(new Vector2(
                    Random.Range(widthDeviation, Screen.width - widthDeviation),
                    Random.Range(heightDeviation, Screen.height - heightDeviation)));
            ShootInDirection(ray);
        }

        protected void ShootQueue(Action shootAction, int bulletCount) {
            IEnumerator ShootRepeatedly() {
                while (bulletCount > 0) {
                    shootAction();
                    bulletCount--;
                    yield return new WaitForSeconds(setting.FireDelay);
                }
            }

            StartCoroutine(ShootRepeatedly());
        }

        protected void ShootBurst(int spreadAngle, int bulletsPerShoot) {
            if (InFireDelay || CurrentAmmoCount <= 0) return;
            
            var widthDeviation = Screen.width / 2f * (1f - spreadAngle / 90f);
            var heightDeviation = Screen.height / 2f * (1f - spreadAngle / 90f);
            
            while (bulletsPerShoot > 0) {
                var ray = _screenPointToRay(
                    new Vector2(
                        Random.Range(widthDeviation, Screen.width - widthDeviation),
                        Random.Range(heightDeviation, Screen.height - heightDeviation)));
                
                if (Physics.Raycast(ray, out var hit, setting.MaxDistance)) {
                    if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable))
                        damageable.Damage(setting.GetWeaponDamage(hit.distance));
                }
            
                var bulletDirection = ray.direction.normalized;
                var bullet = Instantiate(bulletPrefab, shootOrigin.position, Quaternion.LookRotation(bulletDirection));
                bullet.AddForce(bulletDirection, setting.BulletSpeed);
                
                bulletsPerShoot--;
            }

            InFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(setting.FireDelay));
            CurrentAmmoCount--;
        }
        
        public void Reload() {
            InFireDelay = true;
            CurrentAmmoCount = setting.MaxAmmo;
            StartCoroutine(RemoveFireDelayLater(setting.ReloadTime));
        }

        protected IEnumerator RemoveFireDelayLater(float time) {
            yield return new WaitForSeconds(time);
            InFireDelay = false;
        }
    }
}