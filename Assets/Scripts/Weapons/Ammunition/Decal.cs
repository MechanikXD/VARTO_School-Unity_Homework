using UnityEngine;

namespace Weapons.Ammunition
{
    public class Decal : MonoBehaviour
    {
        [SerializeField] private int _liveTime;
        
        private void Start()
        {
            Destroy(gameObject, _liveTime);
        }
    }
}