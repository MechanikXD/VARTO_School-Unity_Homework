using UnityEngine;

namespace Core.Behaviour.ObjectPool
{
    public class ObjectPoolItem<T> where T : Object
    {
        private readonly ObjectPool<T> _sourcePool;
        public T Item { get; }

        public ObjectPoolItem(ObjectPool<T> source, T item)
        {
            _sourcePool = source;
            Item = item;
        }

        public void Return()
        {
            _sourcePool.AcceptReturnee(this);
        }
    }
}