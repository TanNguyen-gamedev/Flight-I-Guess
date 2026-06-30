using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FlightIGuess.Weapons.Core;
using SysNum = System.Numerics;
using FlightIGuess.Core;

namespace FlightIGuess.Weapons.Unity
{
    [System.Serializable]
    public struct EffectPoolConfig
    {
        public string EffectId;
        public PooledEffect EffectPrefab;
        public int DefaultCapacity;
        public int MaxSize; 
    }

    public class EffectPoolManager : MonoBehaviour, IEffectSpawner
    {
        [SerializeField] private EffectPoolConfig[] _effectPoolConfigs;

        private Dictionary<string, IObjectPool<PooledEffect>> _effectPools;

        private void Awake()
        {
            _effectPools = new Dictionary<string, IObjectPool<PooledEffect>>();
            var poolManager = Bootstrapper.Instance.GetManager<PoolManager>();
            if(poolManager == null)
            {
                Debug.LogError("PoolManager not found in the scene!");
                return;
            }
            poolManager.RegisterPool(this);

            foreach (var config in _effectPoolConfigs)
            {
                var currentConfig = config;

                var pool = new ObjectPool<PooledEffect>(
                    createFunc: () => 
                    {
                        var effect = Instantiate(currentConfig.EffectPrefab);
                        return effect;
                    },
                    actionOnGet: (effect) => effect.gameObject.SetActive(true),
                    actionOnRelease: (effect) => effect.gameObject.SetActive(false),
                    actionOnDestroy: (effect) => Destroy(effect.gameObject),
                    collectionCheck: false,
                    defaultCapacity: currentConfig.DefaultCapacity,
                    maxSize: currentConfig.MaxSize
                );

                _effectPools.Add(currentConfig.EffectId, pool);
            }
        }

        private void OnEnable()
        {
            EventBus.Subscribe<SpawnEffectEvent>(OnSpawnEffectEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<SpawnEffectEvent>(OnSpawnEffectEvent);
        }

        private void OnSpawnEffectEvent(SpawnEffectEvent evt)
        {
            Spawn(evt.EffectId, evt.Position, evt.Direction);
        }

        public void Spawn(string effectId, SysNum.Vector2 position, SysNum.Vector2 direction)
        {
            if (_effectPools.TryGetValue(effectId, out var effectPool))
            {
                var effect = effectPool.Get();
                
                effect.SetPool(effectPool);
                
                effect.transform.position = new Vector3(position.X, position.Y, 0f);
                
                var unityDirection = new UnityEngine.Vector2(direction.X, direction.Y);
                effect.Fire(unityDirection);
            }
            else
            {
                Debug.LogWarning($"[EffectPoolManager] No pool configured for Effect ID: {effectId}");
            }
        }
    }
}