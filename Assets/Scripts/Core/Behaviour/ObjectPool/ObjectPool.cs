using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Object = UnityEngine.Object;

namespace Core.Behaviour.ObjectPool
{
    public class ObjectPool<T> where T : Object
    {
        private T _original;
        private ObjectPoolHandlingMode _handlingMode;

        private Stack<ObjectPoolItem<T>> _pool;
        private LinkedList<ObjectPoolItem<T>> _inUse;

        [CanBeNull] private Action<T> _onGet;
        [CanBeNull] private Action<T> _onReturn;

        public int Capacity { get; private set; }

        private const float POOL_REFILL_PERCENT = 0.25f;
        private const float POOL_EXPAND_FACTOR = 1.5f;

        private readonly static
            Dictionary<ObjectPoolHandlingMode, Func<ObjectPool<T>, ObjectPoolItem<T>>>
            OverflowHandlers =
                new Dictionary<ObjectPoolHandlingMode,
                    Func<ObjectPool<T>, ObjectPoolItem<T>>>
                {
                    // Create new and give it
                    [ObjectPoolHandlingMode.CreateInstances] = pool =>
                    {
                        var newInstance = pool.CreateNew(true);
                        return newInstance;
                    },
                    // Increase current size of the pool and refill it
                    [ObjectPoolHandlingMode.ExpandPool] = pool =>
                    {
                        pool.ExpandPool((int)(pool.Capacity * POOL_EXPAND_FACTOR));
                        return pool.Get();
                    },
                    // Create new instances in existing object pool
                    [ObjectPoolHandlingMode.RefillPool] = pool =>
                    {
                        var count = (int)(pool.Capacity * POOL_REFILL_PERCENT);
                        for (var i = 0; i < count; i++)
                        {
                            var newInstance = pool.CreateNew();
                            pool.AddWithoutNotify(newInstance);
                        }

                        return pool.Get();
                    },
                    // Take objects that are being used (Prioritise oldest one's)  
                    [ObjectPoolHandlingMode.ReuseExisting] = pool => pool.ReuseOldest()
                };

        /// <summary>
        /// Gives an instance of an Object to use bound to this pool
        /// </summary>
        public ObjectPoolItem<T> Get()
        {
            if (_pool.Count > 0)
            {
                var value = _pool.Pop();
                _inUse.AddLast(value);
                _onGet?.Invoke(value.Item);
                return value;
            }

            return OverflowHandlers[_handlingMode](this);
        }

        /// <summary>
        /// Releases value from object pool to be use on it's own
        /// </summary>
        /// <param name="value"> Object you want to release </param>
        /// <returns> Released Object what no longer bound to this object pool </returns>
        public T Release(ObjectPoolItem<T> value)
        {
            var data = value.Item;
            _inUse.Remove(value);
            if (_pool.Count >= Capacity) return data; // No need in new instances

            value = new ObjectPoolItem<T>(this, Object.Instantiate(_original));
            _pool.Push(value);
            return data;
        }

        /// <summary>
        /// Destroys all instances of Objects in this pool 
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _pool)
            {
                Object.Destroy(obj.Item);
            }
            _pool.Clear();
            
            foreach (var obj in _inUse)
            {
                _onGet?.Invoke(obj.Item);
                Object.Destroy(obj.Item);
            }
            _inUse.Clear();
            _original = null;
            Capacity = 0;
        }

        /// <summary>
        /// Initialize function must be called before using object pool
        /// </summary>
        /// <param name="copycat"> Object to be copied from </param>
        /// <param name="capacity"> Max capacity of this pool </param>
        /// <param name="onGet"> Method or function that will be called alongside Get() on objects </param>
        /// <param name="onReturn"> Method or function that will be called alongside Return() on objects </param>
        /// <param name="handlingMode"> How object pool will handle "overflow" state </param>
        public void Initialize(T copycat, int capacity, Action<T> onGet=null, Action<T> onReturn=null,
            ObjectPoolHandlingMode handlingMode = ObjectPoolHandlingMode.ExpandPool)
        {
            _original = copycat;
            _onGet = onGet;
            _onReturn = onReturn;
            _pool = new Stack<ObjectPoolItem<T>>(capacity);
            ExpandPool(capacity);
            _inUse = new LinkedList<ObjectPoolItem<T>>();
            _handlingMode = handlingMode;
        }

        /// Increase current capacity of object pool
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

        /// Method that ObjectPoolItem will be calling to return themself into pull 
        internal void AcceptReturnee(ObjectPoolItem<T> item)
        {
            _inUse.Remove(item);

            if (_pool.Count == Capacity)
            {
                Object.Destroy(item.Item);
            }
            else
            {
                _onReturn?.Invoke(item.Item);
                _pool.Push(item);
            }
        }
        
        private void AddWithoutNotify(ObjectPoolItem<T> item)
        {
            if (_pool.Count < Capacity) _pool.Push(item);
        }

        private ObjectPoolItem<T> ReuseOldest()
        {
            var oldest = _inUse.First;
            _inUse.RemoveFirst();
            _inUse.AddLast(oldest);
            _onGet?.Invoke(oldest.Value.Item);
            return oldest.Value;
        }

        private ObjectPoolItem<T> CreateNew(bool callOnGet=false)
        {
            var newInstance =  new ObjectPoolItem<T>(this, Object.Instantiate(_original));
            _inUse.AddLast(newInstance);
            if (callOnGet) _onGet?.Invoke(newInstance.Item);
            return newInstance;
        }
    }
}