using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Behaviour.ObjectPool
{
    public class ObjectPool<T> where T : Object
    {
        private T _original;
        private ObjectPoolHandlingMode _handlingMode;
        private bool _recordUsed;

        private Stack<T> _pool;
        private LinkedList<T> _inUse;

        [CanBeNull] private Action<T> _onGet;
        [CanBeNull] private Action<T> _onReturn;

        public int Capacity { get; private set; }

        private const float POOL_REFILL_PERCENT = 0.25f;
        private const float POOL_EXPAND_FACTOR = 1.5f;

        private readonly static
            Dictionary<ObjectPoolHandlingMode, Func<ObjectPool<T>, T>>
            OverflowHandlers =
                new Dictionary<ObjectPoolHandlingMode,
                    Func<ObjectPool<T>, T>>
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
                    // Take objects that are being used (Prioritise the oldest one's)  
                    [ObjectPoolHandlingMode.ReuseExisting] = pool => pool.ReuseOldest()
                };

        /// <summary>
        /// Gives an instance of an Object to use bound to this pool
        /// </summary>
        public T Get()
        {
            if (_pool.Count > 0)
            {
                var value = _pool.Pop();
                if (_recordUsed) _inUse.AddLast(value);
                _onGet?.Invoke(value);
                return value;
            }

            return OverflowHandlers[_handlingMode](this);
        }

        /// <summary>
        /// Return value to object pool
        /// </summary>
        /// <param name="value"> Object you want to release </param>
        /// <returns> Released Object what no longer bound to this object pool </returns>
        public void Return(T value)
        {
            if (_recordUsed && !_inUse.Remove(value))
            {
                Debug.LogWarning($"Object {value.name} was not a part of the pool. The value was accepted, but such behaviour may cause issues.");    
            }
            if (_pool.Count >= Capacity) return; // Pool is already full

            _onReturn?.Invoke(value);
            _pool.Push(value);
        }

        /// <summary>
        /// Destroys all instances of Objects in this pool 
        /// </summary>
        public void Clear()
        {
            foreach (var obj in _pool)
            {
                Object.Destroy(obj);
            }
            _pool.Clear();
            
            foreach (var obj in _inUse)
            {
                _onGet?.Invoke(obj);
                Object.Destroy(obj);
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
        /// <param name="recordUsed"> Remember objects that being used </param>
        /// <param name="handlingMode"> How object pool will handle "overflow" state </param>
        public void Initialize(T copycat, int capacity, Action<T> onGet=null, Action<T> onReturn=null,
            bool recordUsed = true, ObjectPoolHandlingMode handlingMode = ObjectPoolHandlingMode.ExpandPool)
        {
            _original = copycat;
            _onGet = onGet;
            _onReturn = onReturn;
            _recordUsed = recordUsed;
            _pool = new Stack<T>(capacity);
            ExpandPool(capacity);
            _inUse = new LinkedList<T>();
            _handlingMode = handlingMode;
        }

        /// Increase current capacity of object pool
        private void ExpandPool(int newCapacity, bool instantiateMissing = true)
        {
            if (newCapacity <= Capacity) return;

            var newStack = new Stack<T>(newCapacity);
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
                    _pool.Push(newObject);
                }
            }

            Capacity = newCapacity;
        }

        private void AddWithoutNotify(T item)
        {
            if (_pool.Count < Capacity) _pool.Push(item);
        }

        private T ReuseOldest()
        {
            if (!_recordUsed)
            {
                Debug.LogError("You trying to reuse objects in pool, but disabled recording of sad objects!");
                return null;
            }
            var oldest = _inUse.First;
            _inUse.RemoveFirst();
            _inUse.AddLast(oldest);
            _onGet?.Invoke(oldest.Value);
            return oldest.Value;
        }

        private T CreateNew(bool callOnGet=false)
        {
            var newInstance = Object.Instantiate(_original);
            if (_recordUsed) _inUse.AddLast(newInstance);
            if (callOnGet) _onGet?.Invoke(newInstance);
            return newInstance;
        }
    }
}