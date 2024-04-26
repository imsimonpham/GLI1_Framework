using UnityEngine;

[System.Serializable]
public class WaveData
{
    [SerializeField] int _enemyCount;
    [SerializeField] private float _enemySpawnRate = 1f;

    public int GetenemyCount() { return _enemyCount; }

    public float GetenemySpawnRate() { return _enemySpawnRate; }
}
