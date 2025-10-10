using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private TextAsset waveFile;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bishopPrefab;

    private WaveTimeline waveTimeline;
    private List<SpawnZone> spawnZones = new();

    private void Awake()
    {
        Assert.IsNotNull(waveFile);
        waveTimeline = WaveTimeline.Read(waveFile);
        Assert.IsNotNull(skeletonPrefab);
        Assert.IsNotNull(bishopPrefab);
    }

    private void Start()
    {
        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        waveTimeline.ManualUpdate();
        foreach ((EnemyType type, int toSpawn) in waveTimeline.GetEnemiesToSpawn())
            if (toSpawn > 0)
                SpawnEnemies(type, toSpawn);
    }

    public void SpawnEnemies(EnemyType type, int numEnemies)
    {
        if (numEnemies <= 0) return;
        List<SpawnZone> activeSpawnZones = spawnZones.Where(spawner => spawner.IsSpawnable()).ToList();
        if (activeSpawnZones.Count == 0) return;

        for (int i = 0; i < numEnemies; ++i)
        {
            int zoneIndex = Random.Range(0, activeSpawnZones.Count());
            Vector3 spawnPoint = activeSpawnZones[zoneIndex].GetRandomPoint();
            SpawnAtPoint(type, spawnPoint);
        }
    }

    private void SpawnAtPoint(EnemyType type, Vector3 point)
    {
        GameObject prefab = type switch {
            EnemyType.Skeleton => skeletonPrefab,
            EnemyType.Bishop => bishopPrefab,
            _ => null
        };
        Assert.IsNotNull(prefab);
        Instantiate(prefab, point, Quaternion.identity);
    }

    private void SpawnBishop(Vector3 point)
    {
        Instantiate(bishopPrefab, point, Quaternion.identity);
    }
}
