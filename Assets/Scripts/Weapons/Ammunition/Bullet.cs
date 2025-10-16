using System.Collections;
using UnityEngine;
using Weapons.Abstract;
using Zenject;

namespace Weapons.Ammunition
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour
    {
        private Coroutine _activeCoroutine;
        private Rigidbody _bulletBody;
        [SerializeField] private bool _destroyOnCollision = true;
        [SerializeField] private float _destroyDelay = 3f;
        [SerializeField] private Decal _decalPrefab;
        
        [SerializeField] private string _message;

        [Inject]
        private void Initialize(string message)
        {
            _message = message;
            _bulletBody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            IEnumerator DisableAfter(float time)
            {
                yield return new WaitForSeconds(time);
                Destroy(gameObject);
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
            var decal = Instantiate(_decalPrefab, collisionPoint.point, decalRotation);
            decal.transform.SetParent(other.transform, true);

            if (_destroyOnCollision) Destroy(gameObject);
        }

        private void OnDisable()
        {
            if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
        }

        public void AddForce(Vector3 direction, float speed) =>
            _bulletBody.AddForce(direction * speed, ForceMode.Impulse);
    }
}