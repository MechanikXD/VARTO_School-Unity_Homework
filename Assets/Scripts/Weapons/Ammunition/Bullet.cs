using UnityEngine;
using Weapons.Abstract;

namespace Weapons.Ammunition {
    [RequireComponent(typeof(Rigidbody))]
    public class Bullet : MonoBehaviour {
        [SerializeField] private Rigidbody bulletBody;
        [SerializeField] private bool destroyOnCollision = true;
        [SerializeField] private float destroyDelay = 3f;
        [Space]
        [SerializeField] private Transform decalPrefab;
        [SerializeField] private float decalDestroyDelay = 5f;

        private void Awake() => Destroy(gameObject, destroyDelay);

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.GetComponent<Bullet>() ||
                other.gameObject.GetComponent<WeaponBase>()) return;
            
            var collisionPoint = other.GetContact(0);
            var decalRotation = Quaternion.LookRotation(collisionPoint.normal);
            var decal = Instantiate(decalPrefab, collisionPoint.point, decalRotation);
            Destroy(decal.gameObject, decalDestroyDelay);
            
            if (destroyOnCollision) Destroy(gameObject);
        }

        public void AddForce(Vector3 direction, float speed) =>
            bulletBody.AddForce(direction * speed, ForceMode.Impulse);
    }
}