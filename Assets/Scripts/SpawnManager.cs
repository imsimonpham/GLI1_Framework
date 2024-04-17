using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<GameObject> _enemyPool = new List<GameObject>();
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private Transform _startPoint;
    private int _enemyPoolSize = 1;
    [SerializeField] private int _enemySpawned;

    private void Start()
    {
        _enemyPool = GenerateEnemyPool(_enemyPoolSize);
        /*StartCoroutine(SpawnRoutine());*/
    }

    IEnumerator SpawnRoutine()
    {
        while (_enemySpawned < _enemyPoolSize)
        {
            RequestEnemySpawn();
            yield return new WaitForSeconds(2f);
        }
    }

    private void Update()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            RequestEnemySpawn();
        }
    }

    GameObject RequestEnemySpawn()
    {
        foreach(var enemy in _enemyPool)
        {
            if(enemy.activeInHierarchy == false)
            {
                enemy.transform.position = _startPoint.position;
                enemy.SetActive(true);
                enemy.GetComponent<AI>().SetupAI();
                _enemySpawned++;
                return enemy;
            }
        }
        //create new enemies when needed
        return GenerateEnemyAsNeeded();
    }

    List<GameObject> GenerateEnemyPool(int enemyCount)
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

    GameObject GenerateEnemyAsNeeded()
    {
        GameObject newEnemy = Instantiate(_enemyPrefab, _startPoint.position, Quaternion.Euler(0, -90, 0));
        newEnemy.transform.parent = _enemyContainer.transform;
        _enemyPool.Add(newEnemy);
        newEnemy.GetComponent<AI>().SetupAI();
        _enemySpawned++;
        return newEnemy;
    }
}
