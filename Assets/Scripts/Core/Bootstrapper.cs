
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
                return (T)manager;
            }
            else
            {
                Debug.LogError($"Manager {typeof(T).Name} not found in Bootstrapper");
                return null;
            }
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
                Debug.LogError($"Manager {typeof(T).Name} already registered in Bootstrapper");
            }
            else
            {
                _managers.Add(typeof(T), manager);
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