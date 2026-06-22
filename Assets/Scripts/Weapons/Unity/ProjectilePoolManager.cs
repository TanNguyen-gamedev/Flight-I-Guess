using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using FlightIGuess.Weapons.Core;
using SysNum = System.Numerics;

namespace FlightIGuess.Weapons.Unity
{
    [System.Serializable]
    public struct ProjectilePoolConfig
    {
        public string ProjectileId;
        public PooledProjectile Prefab;
        public int DefaultCapacity;
        public int MaxSize;
    }

    public class ProjectilePoolManager : MonoBehaviour, IProjectileSpawner
    {
        [SerializeField] private ProjectilePoolConfig[] _poolConfigs;
        [SerializeField] private EffectPoolManager _effectPoolManager;

        private Dictionary<string, IObjectPool<PooledProjectile>> _pools;

        private void Awake()
        {
            _pools = new Dictionary<string, IObjectPool<PooledProjectile>>();

            foreach (var config in _poolConfigs)
            {
                // Capture config for the lambda closures
                var currentConfig = config;

                var pool = new ObjectPool<PooledProjectile>(
                    createFunc: () => 
                    {
                        var projectile = Instantiate(currentConfig.Prefab);
                        // Inject the pool reference so the projectile can release itself
                        return projectile;
                    },
                    actionOnGet: (projectile) => projectile.gameObject.SetActive(true),
                    actionOnRelease: (projectile) => projectile.gameObject.SetActive(false),
                    actionOnDestroy: (projectile) => Destroy(projectile.gameObject),
                    collectionCheck: false,
                    defaultCapacity: currentConfig.DefaultCapacity,
                    maxSize: currentConfig.MaxSize
                );

                _pools.Add(currentConfig.ProjectileId, pool);
            }
        }

        public void Spawn(string projectileId, SysNum.Vector2 position, SysNum.Vector2 direction)
        {
            if (_pools.TryGetValue(projectileId, out var pool))
            {
                var projectile = pool.Get();
                
                // Inject the pool reference immediately upon getting it
                projectile.SetPool(pool);
                
                // Inject the effect spawner so the projectile can spawn explosions
                if (_effectPoolManager != null)
                {
                    projectile.SetEffectSpawner(_effectPoolManager);
                }
                
                projectile.transform.position = new Vector3(position.X, position.Y, 0f);
                
                // Convert System.Numerics.Vector2 to UnityEngine.Vector2
                var unityDirection = new UnityEngine.Vector2(direction.X, direction.Y);
                projectile.Fire(unityDirection);
            }
            else
            {
                Debug.LogWarning($"[ProjectilePoolManager] No pool configured for Projectile ID: {projectileId}");
            }
        }
    }
}