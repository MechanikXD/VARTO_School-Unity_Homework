using System;
using System.Collections;
using System.Collections.Generic;
using Core.Audio;
using Core.Behaviour.ObjectPool;
using Enemy.Damageable;
using Player;
using UnityEngine;
using Weapons.Ammunition;
using Random = UnityEngine.Random;

namespace Weapons.Abstract {
    public class WeaponBase : MonoBehaviour {
        [SerializeField] protected Rigidbody _weaponBody;
        [SerializeField] protected BoxCollider _weaponCollider;
        [SerializeField] protected WeaponSettings _settings;
        [SerializeField] protected Transform _shootOrigin;
        [SerializeField] protected Bullet _bulletPrefab;
        private bool _inFireDelay;
        private int _currentAmmoCount;

        private Dictionary<ShootType, Action> _shootActions;
        private Func<Vector3, Ray> _screenPointToRay;

        private bool _isShooting;
        
        private void Awake() {
            Initialize();
        }

        private void Initialize() {
            _currentAmmoCount = _settings.MaxAmmo;
            _screenPointToRay = Camera.main!.ScreenPointToRay;
            if (!ObjectPoolManager.Contains<Bullet>()) ObjectPoolManager.Create(_bulletPrefab, 50);

            _shootActions = new Dictionary<ShootType, Action>() {
                [ShootType.Straight] = ShootForward,
                [ShootType.WithDeviation] = ShootForwardWithDeviation,
                [ShootType.StraightQueue] = () => ShootQueue(ShootForward),
                [ShootType.QueueWithDeviation] = () => ShootQueue(ShootForwardWithDeviation),
                [ShootType.Burst] = ShootBurst,
                [ShootType.Custom] = () => {}
            };
        }

        #region Shooting

        public void SetCustomShootBehaviour(Action behaviour) {
            _shootActions[ShootType.Custom] = behaviour;
        }

        private void SetContinuousShooting(bool isContinuous) {
            if (isContinuous && !_isShooting) {
                _isShooting = true;

                IEnumerator ShootContinuously() {
                    while (_isShooting) {
                        if (!_inFireDelay) AudioController.Instance.PlaySfx(_shootOrigin.position, _settings.ShootSound);
                        _shootActions[_settings.Type]();
                        yield return new WaitForSeconds(_settings.FireDelay);
                    }
                }

                StartCoroutine(ShootContinuously());
            }
            else if (!isContinuous && _isShooting) {
                _isShooting = false;
            }
        }

        private Ray GetShootRay(float angleDeviation) {
            var widthDeviation = Screen.width / 2f * (1f - angleDeviation / 90f);
            var heightDeviation = Screen.height / 2f * (1f - angleDeviation / 90f);

            return _screenPointToRay(new Vector2(
                Random.Range(widthDeviation, Screen.width - widthDeviation),
                Random.Range(heightDeviation, Screen.height - heightDeviation)));
        }

        public void Shoot(bool isShooting) {
            if (_settings.IsAutomatic) {
                SetContinuousShooting(isShooting);
            }
            else if (_currentAmmoCount > 0) {
                if (!_inFireDelay) AudioController.Instance.PlaySfx(_shootOrigin.position, _settings.ShootSound);
                _shootActions[_settings.Type]();
            }
        }

        private void ShootInDirection(Ray direction) {
            if (_inFireDelay || _currentAmmoCount <= 0) return;

            if (Physics.Raycast(direction, out var hit, _settings.MaxDistance)) {
                if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {
                    damageable.Damage(_settings.GetWeaponDamage(hit.distance));
                }
            }

            ShootBullet(direction);

            _inFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(_settings.FireDelay));
            _currentAmmoCount--;
        }

        private void ShootForward() {
            var forwardRay = _screenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));
            ShootInDirection(forwardRay);
        }

        private void ShootForwardWithDeviation() {
            ShootInDirection(GetShootRay(_settings.DeviationAngle));
        }

        private void ShootQueue(Action shootAction) {
            var bulletCount = _settings.QueueLength;
            bool firstWasShot = false;
            
            IEnumerator ShootRepeatedly() {
                while (bulletCount > 0) {
                    // Workaround existing structure: When using burst sound is played only once
                    // despite bullet being shot several times.
                    if (firstWasShot) firstWasShot = true;
                    else AudioController.Instance.PlaySfx(_shootOrigin.position, _settings.ShootSound);
                    
                    shootAction();
                    bulletCount--;
                    yield return new WaitForSeconds(_settings.FireDelay);
                }
            }

            StartCoroutine(ShootRepeatedly());
        }

        private void ShootBurst() {
            if (_inFireDelay || _currentAmmoCount <= 0) return;

            var palletCount = _settings.BurstPalletCount;
            while (palletCount > 0) {
                var ray = GetShootRay(_settings.DeviationAngle);
                
                if (Physics.Raycast(ray, out var hit, _settings.MaxDistance)) {
                    if (hit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable))
                        damageable.Damage(_settings.GetWeaponDamage(hit.distance));
                }
            
                ShootBullet(ray);
                var bulletDirection = ray.direction.normalized;
                var bullet = Instantiate(_bulletPrefab, _shootOrigin.position, Quaternion.LookRotation(bulletDirection));
                bullet.AddForce(bulletDirection, _settings.BulletSpeed);
                
                palletCount--;
            }

            _inFireDelay = true;
            StartCoroutine(RemoveFireDelayLater(_settings.FireDelay));
            _currentAmmoCount--;
        }

        #endregion

        private void ShootBullet(Ray direction)
        {
            var bullet = ObjectPoolManager.Get<Bullet>().Get();
            bullet.Item.SetObjectPoolItem(bullet);
            
            var bulletDirection = direction.direction.normalized;
            var bulletPosition = _shootOrigin.position;
            
            bullet.Item.gameObject.SetActive(true);
            bullet.Item.transform.position = bulletPosition;
            bullet.Item.transform.rotation = Quaternion.LookRotation(bulletDirection);
            
            bullet.Item.AddForce(bulletDirection, _settings.BulletSpeed);
        }
        
        public void Reload() {
            _inFireDelay = true;
            _currentAmmoCount = _settings.MaxAmmo;
            StartCoroutine(RemoveFireDelayLater(_settings.ReloadTime));
        }

        public void DetachFromParent() {
            _weaponCollider.enabled = true;
            _weaponBody.isKinematic = false;
            _weaponBody.useGravity = true;
            transform.parent = null;
        }

        public void AttachTo(Transform parent) {
            _weaponCollider.enabled = false;
            _weaponBody.isKinematic = true;
            _weaponBody.useGravity = false;

            var thisWeaponTransform = transform;
            thisWeaponTransform.rotation = Quaternion.identity;
            thisWeaponTransform.position = Vector3.zero;
            
            transform.SetParent(parent, false);
        }

        public void Throw(Vector3 force) => _weaponBody.AddForce(force, ForceMode.Impulse);
        public void ThrowForward(float force) => Throw(Vector3.forward * force);

        private IEnumerator RemoveFireDelayLater(float time) {
            yield return new WaitForSeconds(time);
            _inFireDelay = false;
        }
        
        public void OnCollisionEnter(Collision other) {
            if (other.gameObject.TryGetComponent<PlayerController>(out var player)) player.TryAddWeapon(this);
        }
    }
}