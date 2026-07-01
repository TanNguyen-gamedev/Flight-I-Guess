
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlightIGuess.Core
{
    public class Bootstrapper: MonoBehaviour
    {
        public static Bootstrapper Instance { get; private set; }
        
        private readonly Dictionary<Type, MonoBehaviour> _managers = new();
        
        public T GetManager<T>() where T : MonoBehaviour
        {
            if(_managers.TryGetValue(typeof(T), out var manager))
            {
                if (manager == null)
                {
                    Debug.LogWarning($"Manager {typeof(T).Name} is registered but was destroyed. Returning null.");
                    return null;
                }
                return (T)manager;
            }
            else
            {
                Debug.LogError($"Manager {typeof(T).Name} not found in Bootstrapper");
                return null;
            }
        }

        public bool TryGetManager<T>(out T manager) where T : MonoBehaviour
        {
            if(_managers.TryGetValue(typeof(T), out var foundManager))
            {
                if (foundManager == null)
                {
                    manager = null;
                    return false;
                }
                manager = (T)foundManager;
                return true;
            }
            manager = null;
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            var prefab = Resources.Load<Bootstrapper>("Bootstrapper");
            if (prefab != null)
            {
                Instance = Instantiate(prefab);
                DontDestroyOnLoad(Instance.gameObject);
            }
            else
            {
                Debug.LogError("Bootstrapper prefab not found in Resources/Bootstrapper!");
            }
        }

        public void Register<T>(T manager) where T : MonoBehaviour
        {
            if(_managers.TryGetValue(typeof(T), out var existingManager))
            {
                if (existingManager != null && existingManager != manager)
                {
                    Debug.LogError($"Manager {typeof(T).Name} already registered in Bootstrapper");
                }
                else
                {
                    _managers[typeof(T)] = manager;
                }
            }
            else
            {
                _managers.Add(typeof(T), manager);
            }
        }

        public void Unregister<T>(T manager) where T : MonoBehaviour
        {
            if (_managers.TryGetValue(typeof(T), out var existingManager))
            {
                if (existingManager == manager)
                {
                    _managers.Remove(typeof(T));
                }
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (var manager in GetComponentsInChildren<MonoBehaviour>())
            {
                if (manager == this) continue; // Don't add the Bootstrapper itself
                
                var type = manager.GetType();
                if (!_managers.ContainsKey(type))
                {
                    _managers.Add(type, manager);
                }
            }
        }

    }
}