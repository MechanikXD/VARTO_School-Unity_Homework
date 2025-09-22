using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.Behaviour.ObjectPool
{
    public static class ObjectPoolManager
    {
        private readonly static Dictionary<Type, object> Pools = new Dictionary<Type, object>();

        public static ObjectPool<T> Get<T>() where T : Object
        {
            return (ObjectPool<T>)Pools[typeof(T)];
        }
        
        public static bool Contains<T>() where T : Object
        {
            return Pools.ContainsKey(typeof(T));
        }
        
        public static void Create<T>(T copycat, int capacity, 
            ObjectPoolHandlingMode mode = ObjectPoolHandlingMode.ExpandPool) where T : Object
        {
            var newPool = new ObjectPool<T>();
            newPool.Initialize(copycat, capacity, mode);
            Pools.Add(typeof(T), newPool);
        }

        public static void Delete<T>() where T : Object
        {
            Get<T>().Clear();
            Pools.Remove(typeof(T));
        }
    }
}