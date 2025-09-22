using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.Behaviour.ObjectPool
{
    public static class ObjectPoolManager
    {
        private static Dictionary<Type, object> _pools;

        public static ObjectPool<T> Get<T>() where T : Object
        {
            return (ObjectPool<T>)_pools[typeof(T)];
        }
        
        public static bool Contains<T>() where T : Object
        {
            return _pools.ContainsKey(typeof(T));
        }
        
        public static void Create<T>(T copycat, int capacity, 
            ObjectPoolHandlingMode mode = ObjectPoolHandlingMode.ExpandPool) where T : Object
        {
            var newPool = new ObjectPool<T>();
            newPool.Initialize(copycat, capacity, mode);
            _pools.Add(typeof(T), newPool);
        }

        public static void Delete<T>() where T : Object
        {
            Get<T>().Clear();
            _pools.Remove(typeof(T));
        }
    }
}