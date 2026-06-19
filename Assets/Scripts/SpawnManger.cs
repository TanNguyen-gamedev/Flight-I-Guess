using System.Collections;
using UnityEngine;

public class SpawnManger : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private float _burstSpawnInterval = 5f;
    [SerializeField] private int _burstNumber = 3;
    [SerializeField] private Transform _leftAnchor;
    [SerializeField] private Transform _rightAnchor;
    private bool _isGameOver = false;


    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }


    private IEnumerator SpawnCoroutine()
    {
        while(!_isGameOver)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private IEnumerator BurstSpawnCoroutine()
    {
        while(!_isGameOver)
        {
            
            yield return new WaitForSeconds(_burstSpawnInterval);
        }
    }
    
    private void SpawnEnemy()
    {
        Vector2 randomPos = new Vector2(
            Random.Range(_leftAnchor.position.x, _rightAnchor.position.x),
            Random.Range(_leftAnchor.position.y, _rightAnchor.position.y)
        );
        Instantiate(_enemyPrefab[Random.Range(0, _enemyPrefab.Length)], randomPos, Quaternion.identity);
    }


}
