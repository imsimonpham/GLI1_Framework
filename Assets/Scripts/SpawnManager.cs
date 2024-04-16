using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<GameObject> _enemyPool = new List<GameObject>();
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private Transform _startPoint;
    private int _enemyPoolSize = 20;
    [SerializeField] private int _enemySpawned = 0;

    private void Start()
    {
        _enemyPool = SpawnEnemies(_enemyPoolSize);
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (_enemySpawned < _enemyPoolSize)
        {
            RequestEnemySpawn();
            yield return new WaitForSeconds(2f);
        }
    }

    GameObject RequestEnemySpawn()
    {
        foreach(var enemy in _enemyPool)
        {
            if(enemy.activeInHierarchy == false)
            {
                enemy.SetActive(true);
                _enemySpawned++;
                return enemy;
            }
        }
        //TODO: Dynamically spawn new enemies 
        return null;
    }

    List<GameObject> SpawnEnemies(int enemyCount)
    {
        for(var i = 0; i < enemyCount; i++)
        {
            GameObject enemyGO = Instantiate(_enemyPrefab, _startPoint.position, Quaternion.Euler(0, -90, 0));
            enemyGO.transform.parent = _enemyContainer.transform;
            enemyGO.SetActive(false);
            _enemyPool.Add(enemyGO);    
        }
        return _enemyPool;
    }
}
