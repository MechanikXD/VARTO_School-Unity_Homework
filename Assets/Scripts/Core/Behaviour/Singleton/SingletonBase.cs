using UnityEngine;

namespace Core.Behaviour.Singleton
{
    public class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private bool _dontDestroyOnLoad;
        protected T Instance;

        private void Awake()
        {
            ToSingleton();
            Initialize();
        }

        private void OnDestroy()
        {
            BeforeDestroy();
            
            if (Instance == this) {
                Instance = null;
            }
        }

        private void ToSingleton()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"Multiple Instances of {typeof(T)} was found on the scene!\n" +
                                 $"{gameObject.name} will be destroyed upon start.");
                Destroy(this.gameObject);
            }

            Instance = (T)(MonoBehaviour)this;
            if (_dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        }

        protected virtual void Initialize() { }
        protected virtual void BeforeDestroy() { }
    }
}