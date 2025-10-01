using UnityEngine;
using Zenject;

namespace Other
{
    public class GreeterSpawner : MonoBehaviour
    {
        // To force injection -> spawn Greeter object on the scene.
        [Inject] private Greeter _toSpawn;
    }
}