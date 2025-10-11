using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnWaveUIController uiController;
    [SerializeField] private TextAsset waveFile;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bishopPrefab;

    private WaveTimeline waveTimeline;
    private List<SpawnZone> spawnZones = new();

    private void Awake()
    {
        Assert.IsNotNull(uiController);
        Assert.IsNotNull(waveFile);
        waveTimeline = WaveTimeline.Read(waveFile);
        Assert.IsNotNull(skeletonPrefab);
        Assert.IsNotNull(bishopPrefab);

        waveTimeline.onWaveNumberChanged.AddListener(OnWaveNumberChanged);
        waveTimeline.Init();
    }

    private void Start()
    {
        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
    }

    private void Update()
    {
        waveTimeline.ManualUpdate();
        uiController.SetNormalizedSpawningTimeLeft(waveTimeline.GetNormalizedSpawningTimeLeft());
        uiController.SetNormalizedWaitTime(waveTimeline.GetNormalizedWaitTime());
        foreach ((EnemyType type, int toSpawn) in waveTimeline.GetEnemiesToSpawn()) SpawnEnemies(type, toSpawn);
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

    private void OnWaveNumberChanged(int waveNumber)
    {
        if (waveNumber <= waveTimeline.NumberOfWaves())
            uiController.SetWaveNumber(waveNumber);
        else
            uiController.HideUI();
    }
}
