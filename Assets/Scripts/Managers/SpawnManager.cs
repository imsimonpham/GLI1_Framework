using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    //Object Pooling + Spawn enemies
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private List<GameObject> _enemyPool = new List<GameObject>();
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private Transform _startPoint;
    private int _enemyPoolSize = 1;

    //Wave Spawner
    [SerializeField] private WaveData[] _waveData;
    [SerializeField] private int _currentWaveIndex;
    [SerializeField] private int _nextWaveIndex;
    [SerializeField] private bool _startNextWave;
    [SerializeField] private int _enemiesSpawnedInCurrentWave;
    [SerializeField] private int _enemiesKilledInCurrentWave;
    [SerializeField] private int _totalEnemiesAlive;
    [SerializeField] private int _totalEnemiesSpawned;
    [SerializeField] private int _totalEnemiesToSpawn;
    private bool _showingNextWaveText;
    private bool _didPlayerSurvive;
    

    private void Start()
    {
        _enemyPool = GenerateEnemyPool(_enemyPoolSize);
        _currentWaveIndex = 0;
        _startNextWave = true;

        foreach(WaveData waveData in _waveData)
        {
            _totalEnemiesToSpawn += waveData.GetenemyCount();
        }
    }

    private void Update()
    {
        if (_showingNextWaveText)
        {
            UIManager.Instance.ShowNextWaveText(true);
            _showingNextWaveText = false;
        }
        else
        {
            UIManager.Instance.ShowNextWaveText(false);
            _showingNextWaveText = false;
        }

        if (_startNextWave)
        {
            _showingNextWaveText = true;
            if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                _showingNextWaveText = false;
                _enemiesSpawnedInCurrentWave = 0;
                _enemiesKilledInCurrentWave = 0;
                _currentWaveIndex = _nextWaveIndex;
                StartCoroutine(SpawnWaveRoutine());
                _startNextWave = false;
            } 
        }

        if(_enemiesKilledInCurrentWave >= _enemiesSpawnedInCurrentWave * 0.5f || _totalEnemiesAlive <= 2)
        {
            _startNextWave = true;
        }

        if(_currentWaveIndex == _waveData.Length - 1)
        {
            _startNextWave = false;
        }
    }

    IEnumerator SpawnWaveRoutine()
    {
        
        WaveData waveData = _waveData[_currentWaveIndex];
        UIManager.Instance.UpdateWaveText(_currentWaveIndex + 1, _waveData.Length);
        for (var i = 0; i < waveData.GetenemyCount(); i++)
        {
            RequestEnemySpawn();
            _totalEnemiesAlive++;
            _enemiesSpawnedInCurrentWave++;
            yield return new WaitForSeconds(waveData.GetenemySpawnRate());
        }
        if(_currentWaveIndex < _waveData.Length - 1)
        {
            _nextWaveIndex = _currentWaveIndex  + 1;
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
                enemy.GetComponent<AI>().SetEnemyWaveIndex(_currentWaveIndex);
                _totalEnemiesSpawned++;
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
        newEnemy.GetComponent<AI>().SetEnemyWaveIndex(_currentWaveIndex);
        _totalEnemiesSpawned++;
        return newEnemy;
    }

    public void UpdateEnemyKilledCount(AI enemy)
    {
        if (enemy.GetEnemyWaveIndex() == _currentWaveIndex)
        {
            _enemiesKilledInCurrentWave++;
        }
    }

    public void UpdateEnemiesAlive()
    {
        _totalEnemiesAlive--;
    }

    public int GetTotalEnemiesAlive()
    {
        return _totalEnemiesAlive;
    }

    public bool FinishedAllWaves()
    {
        if(_currentWaveIndex == _waveData.Length - 1)
        {
            return true;
        }else
        {
            return false;
        }
    }

    public int GetTotalEnemiesSpawned()
    {
        return _totalEnemiesSpawned;
    }

    public bool DidPlayerSurvive()
    {
        if(_totalEnemiesToSpawn == _totalEnemiesSpawned && _totalEnemiesAlive == 0)
        {
            _didPlayerSurvive = true;
        }
        return _didPlayerSurvive;
    }
}
