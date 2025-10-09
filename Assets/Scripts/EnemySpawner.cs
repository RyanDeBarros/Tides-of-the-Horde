using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float spawnRate = 3f;
    [SerializeField] private GameObject skeletonPrefab;

    private List<SpawnZone> spawnZones = new();

    private float spawnDebt = 0f;

    private void Awake()
    {
        Assert.IsNotNull(skeletonPrefab);
    }

    private void Start()
    {
        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        spawnDebt += Time.deltaTime;
        int toSpawn = (int)(spawnDebt * spawnRate);
        spawnDebt -= toSpawn / spawnRate;

        if (toSpawn > 0) SpawnEnemies(toSpawn);
    }

    public void SpawnEnemies(int numEnemies)
    {
        List<SpawnZone> activeSpawnZones = spawnZones.Where(spawner => spawner.IsSpawnable()).ToList();
        if (activeSpawnZones.Count == 0) return;

        for (int i = 0; i < numEnemies; ++i)
        {
            int zoneIndex = Random.Range(0, activeSpawnZones.Count());
            Vector3 spawnPoint = activeSpawnZones[zoneIndex].GetRandomPoint();
            SpawnAtPoint(spawnPoint);
        }
    }

    private void SpawnAtPoint(Vector3 point)
    {
        SpawnSkeleton(point);
    }

    private void SpawnSkeleton(Vector3 point)
    {
        Instantiate(skeletonPrefab, point, Quaternion.identity);
    }
}
