using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.Behaviour.ObjectPool
{
    public class ObjectPool<T> where T : Object
    {
        private T _original;
        private ObjectPoolOverflowHandlingMode _overflowHandlingMode;

        private Stack<ObjectPoolItem<T>> _pool;
        private LinkedList<ObjectPoolItem<T>> _inUse;

        public int Capacity { get; private set; }

        private const float POOL_REFILL_PERCENT = 0.25f;
        private const float POOL_EXPAND_FACTOR = 1.5f;

        private readonly static
            Dictionary<ObjectPoolOverflowHandlingMode, Func<ObjectPool<T>, ObjectPoolItem<T>>>
            OverflowHandlers =
                new Dictionary<ObjectPoolOverflowHandlingMode,
                    Func<ObjectPool<T>, ObjectPoolItem<T>>>
                {
                    [ObjectPoolOverflowHandlingMode.CreateInstances] = pool => pool.CreateNew(),
                    [ObjectPoolOverflowHandlingMode.ExpandPool] = pool =>
                    {
                        pool.ExpandPool((int)(pool.Capacity * POOL_EXPAND_FACTOR));
                        return pool.Get();
                    },
                    [ObjectPoolOverflowHandlingMode.RefillPool] = pool =>
                    {
                        var count = (int)(pool.Capacity * POOL_REFILL_PERCENT);
                        for (var i = 0; i < count; i++)
                        {
                            var newInstance = pool.CreateNew();
                            pool.AddWithoutNotify(newInstance);
                        }

                        return pool.Get();
                    },
                    [ObjectPoolOverflowHandlingMode.ReuseExisting] = pool =>
                    {
                        var latest = pool._inUse.First;
                        pool._inUse.RemoveFirst();
                        return latest.Value;
                    }
                };

        public ObjectPoolItem<T> Get()
        {
            if (_pool.Count > 0)
            {
                var value = _pool.Pop();
                _inUse.AddLast(value);
                return value;
            }

            return OverflowHandlers[_overflowHandlingMode](this);
        }

        public T Release(ObjectPoolItem<T> value)
        {
            var data = value.Item;
            var newItem = new ObjectPoolItem<T>(this, Object.Instantiate(_original));
            if (_pool.Count < Capacity) _pool.Push(newItem);
            return data;
        }

        public void Initialize(T copycat, int capacity,
            ObjectPoolOverflowHandlingMode overflowHandlingMode =
                ObjectPoolOverflowHandlingMode.ExpandPool)
        {
            _original = copycat;
            _pool = new Stack<ObjectPoolItem<T>>(capacity);
            ExpandPool(capacity);
            _inUse = new LinkedList<ObjectPoolItem<T>>();
            _overflowHandlingMode = overflowHandlingMode;
        }

        private void ExpandPool(int newCapacity, bool instantiateMissing = true)
        {
            if (newCapacity <= Capacity) return;

            var newStack = new Stack<ObjectPoolItem<T>>(newCapacity);
            foreach (var value in _pool)
            {
                newStack.Push(value);
            }

            _pool = newStack;

            if (instantiateMissing)
            {
                for (var i = 0; i < newCapacity - Capacity; i++)
                {
                    var newObject = Object.Instantiate(_original);
                    var item = new ObjectPoolItem<T>(this, newObject);
                    _pool.Push(item);
                }
            }

            Capacity = newCapacity;
        }

        internal void AcceptReturnee(ObjectPoolItem<T> item)
        {
            _inUse.Remove(item);

            if (_pool.Count == Capacity)
            {
                Object.Destroy(item.Item);
            }
            else
            {
                _pool.Push(item);
            }
        }

        private void AddWithoutNotify(ObjectPoolItem<T> item)
        {
            if (_pool.Count < Capacity) _pool.Push(item);
        }

        private ObjectPoolItem<T> CreateNew() =>
            new ObjectPoolItem<T>(this, Object.Instantiate(_original));
    }
}