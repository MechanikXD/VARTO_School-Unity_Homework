using UnityEngine;
using Zenject;

namespace Other
{
    public class Greeter : MonoBehaviour
    {
        [Inject] private string _message;
        // This thing will spawn on startup (Injected in Spawner as new, empty GameObject)
        // And start will display injected message
        private void Start()
        {
            Debug.Log(_message);
        }
    }
}