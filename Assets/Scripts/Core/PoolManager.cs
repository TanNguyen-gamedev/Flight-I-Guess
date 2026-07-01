
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightIGuess.Core
{
    public interface IClearablePool
    {
        void ClearActiveObjects();
    }

    public class PoolManager : MonoBehaviour
    {
        private readonly Dictionary<Type, MonoBehaviour> _poolManagers = new();

        public void RegisterPool<T>(T poolManager) where T : MonoBehaviour
        {
            _poolManagers[typeof(T)] = poolManager;
        }
        
        public T GetPool<T>() where T : MonoBehaviour
        {
            if (_poolManagers.TryGetValue(typeof(T), out MonoBehaviour poolManager))
            {
                return poolManager as T;
            }
            else
            {
                Debug.LogError($"PoolManager: PoolManager for type {typeof(T)} not found!");
                return null;
            }
        }

        public void ClearAllPools()
        {
            foreach (var keyValuePair in _poolManagers)
            {
                if (keyValuePair.Value is IClearablePool clearable)
                {
                    clearable.ClearActiveObjects();
                }
            }
        }
    }
}