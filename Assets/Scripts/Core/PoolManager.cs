
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightIGuess.Core
{
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
    }
}