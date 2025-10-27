using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

class OnDestroyHandler : MonoBehaviour
{
    public System.Action<GameObject> onDestroyed;

    private void OnDestroy()
    {
        onDestroyed?.Invoke(gameObject);
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnWaveUIController uiController;
    [SerializeField] private TextAsset waveFile;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ChallengeGiver challengeGiver;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject orcPrefab;

    private WaveTimeline waveTimeline;
    private List<SpawnZone> spawnZones;
    private readonly HashSet<GameObject> spawnedEnemies = new();

    private enum LevelPhase
    {
        ChallengeGiver,
        Waves
    }

    private LevelPhase levelPhase = LevelPhase.ChallengeGiver;

    private void Awake()
    {
        Assert.IsNotNull(uiController);
        Assert.IsNotNull(waveFile);
        waveTimeline = WaveTimeline.Read(waveFile);
        Assert.IsNotNull(shopUI);
        Assert.IsNotNull(challengeGiver);
        challengeGiver.onConversationEnd.AddListener(StartWaves);

        Assert.IsNotNull(skeletonPrefab);
        Assert.IsNotNull(bishopPrefab);
        Assert.IsNotNull(orcPrefab);
    }

    private void Start()
    {
        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
        uiController.HideUI();

        StartCoroutine(StartLevelRoutine());
    }

    private IEnumerator StartLevelRoutine()
    {
        yield return new WaitForSeconds(1f);
        StartLevel();
    }

    private void StartLevel()
    {
        challengeGiver.SpawnNPC();
    }

    private void Update()
    {
        if (levelPhase == LevelPhase.Waves)
            UpdateSpawnTimeline();
    }

    private void StartWaves()
    {
        uiController.ShowUI();
        waveTimeline.onWaveNumberChanged = OnWaveNumberChanged;
        waveTimeline.doEnemiesRemain = DoEnemiesRemain;
        waveTimeline.Init();
        levelPhase = LevelPhase.Waves;
    }

    private void UpdateSpawnTimeline()
    {
        waveTimeline.ManualUpdate();
        uiController.SetNormalizedSpawningTimeLeft(waveTimeline.GetNormalizedSpawningTimeLeft());
        uiController.SetNormalizedWaitTime(waveTimeline.GetNormalizedWaitTime());

        foreach (((EnemyType type, int difficultyLevel), int toSpawn) in waveTimeline.GetEnemiesToSpawn())
            SpawnEnemies(type, toSpawn, difficultyLevel);
    }

    public void SpawnEnemies(EnemyType type, int numEnemies, int difficultyLevel)
    {
        if (numEnemies <= 0) return;
        List<SpawnZone> activeSpawnZones = spawnZones.Where(spawner => spawner.IsSpawnable()).ToList();
        if (activeSpawnZones.Count == 0) return;

        for (int i = 0; i < numEnemies; ++i)
        {
            int zoneIndex = Random.Range(0, activeSpawnZones.Count());
            Vector3 spawnPoint = activeSpawnZones[zoneIndex].GetRandomPoint();
            SpawnAtPoint(type, spawnPoint, difficultyLevel);
        }
    }

    private void SpawnAtPoint(EnemyType type, Vector3 point, int difficultyLevel)
    {
        GameObject instance = Instantiate(GetEnemyPrefab(type), point, Quaternion.identity);
        spawnedEnemies.Add(instance);
        instance.AddComponent<OnDestroyHandler>().onDestroyed = go => spawnedEnemies.Remove(go);
        if (instance.TryGetComponent(out IDifficultyImplementer difficulty))
            difficulty.SetDifficultyLevel(difficultyLevel);
    }

    private GameObject GetEnemyPrefab(EnemyType type)
    {
        GameObject prefab = type switch
        {
            EnemyType.Skeleton => skeletonPrefab,
            EnemyType.Bishop => bishopPrefab,
            EnemyType.Orc => orcPrefab,
            _ => null
        };
        Assert.IsNotNull(prefab);
        return prefab;
    }

    private void OnWaveNumberChanged(int waveNumber)
    {
        if (waveNumber <= waveTimeline.NumberOfWaves())
        {
            uiController.SetWaveNumber(waveNumber);
            shopUI.RefreshOptions();
        }
        else
            uiController.HideUI();
    }

    private bool DoEnemiesRemain()
    {
        return spawnedEnemies.Count > 0;
    }
}
