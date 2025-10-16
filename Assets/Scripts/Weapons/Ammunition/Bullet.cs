using System.Collections;
using Core.Behaviour.ObjectPool;
using UnityEngine;
using Weapons.Abstract;

namespace Weapons.Ammunition
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Rigidbody _bulletBody;
        [SerializeField] private bool _destroyOnCollision = true;
        [SerializeField] private float _destroyDelay = 3f;

        [Space]
        [SerializeField] private Transform decalPrefab;
        [SerializeField] private float _decalDestroyDelay = 5f;
        private Coroutine _activeCoroutine;

        private void Awake()
        {
            _bulletBody = GetComponent<Rigidbody>();
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            IEnumerator DisableAfter(float time)
            {
                yield return new WaitForSeconds(time);
                if (gameObject.activeInHierarchy) gameObject.SetActive(false);
                _activeCoroutine = null;
            }

            _activeCoroutine = StartCoroutine(DisableAfter(_destroyDelay));
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponent<Bullet>() ||
                other.gameObject.GetComponent<WeaponBase>()) return;

            var collisionPoint = other.GetContact(0);
            var decalRotation = Quaternion.LookRotation(collisionPoint.normal);
            var decal = Instantiate(decalPrefab, collisionPoint.point, decalRotation);
            decal.SetParent(other.transform, true);
            Destroy(decal.gameObject, _decalDestroyDelay);

            if (_destroyOnCollision) gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
            ObjectPoolManager.Get<Bullet>().Return(this);
        }

        public void AddForce(Vector3 direction, float speed) =>
            _bulletBody.AddForce(direction * speed, ForceMode.Impulse);
    }
}