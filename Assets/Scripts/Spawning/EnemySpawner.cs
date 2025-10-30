using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

class OnDestroyHandler : MonoBehaviour
{
    public readonly UnityEvent<GameObject> onDestroyed = new();

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
    [SerializeField] private ChallengeTracker challengeTracker;

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject orcPrefab;
    [SerializeField] private GameObject demonKingPrefab;

    [Min(0)] public int difficultyLevelOffset = 0;

    private WaveTimeline waveTimeline;
    private List<SpawnZone> spawnZones;
    private List<Waypoint> waypoints;
    private readonly HashSet<GameObject> spawnedEnemies = new();

    private enum LevelPhase
    {
        ChallengeGiver,
        Waves
    }

    private LevelPhase levelPhase = LevelPhase.ChallengeGiver;

    public void Initialize(TextAsset waveFile)
    {
        this.waveFile = waveFile;
    }

    private void Awake()
    {
        Assert.IsNotNull(uiController);
        Assert.IsNotNull(shopUI);
        Assert.IsNotNull(challengeGiver);
        challengeGiver.onConversationEnd.AddListener(StartWaves);
        Assert.IsNotNull(challengeTracker);

        Assert.IsNotNull(skeletonPrefab);
        Assert.IsNotNull(bishopPrefab);
        Assert.IsNotNull(orcPrefab);
        Assert.IsNotNull(demonKingPrefab);

        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
        waypoints = new(FindObjectsByType<Waypoint>(FindObjectsSortMode.InstanceID));
    }

    private void Start()
    {
        Assert.IsNotNull(waveFile);
        waveTimeline = WaveTimeline.Read(waveFile);

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

        for (int _ = 0; _ < numEnemies; ++_)
        {
            Vector3 spawnPoint = activeSpawnZones.GetRandomElement().GetRandomPoint();
            SpawnAtPoint(type, spawnPoint, difficultyLevel);
        }
    }

    private void SpawnAtPoint(EnemyType type, Vector3 point, int difficultyLevel)
    {
        GameObject instance = Instantiate(GetEnemyPrefab(type), point, Quaternion.identity);
        spawnedEnemies.Add(instance);
        instance.AddComponent<OnDestroyHandler>().onDestroyed.AddListener(go => spawnedEnemies.Remove(go));
        if (instance.TryGetComponent(out IDifficultyImplementer difficulty))
            difficulty.SetDifficultyLevel(difficultyLevel + difficultyLevelOffset);
        if (instance.TryGetComponent(out WaypointPatroller waypointPatroller))
            waypointPatroller.waypoints = waypoints;
    }

    private GameObject GetEnemyPrefab(EnemyType type)
    {
        GameObject prefab = type switch
        {
            EnemyType.Skeleton => skeletonPrefab,
            EnemyType.Bishop => bishopPrefab,
            EnemyType.Orc => orcPrefab,
            EnemyType.DemonKing => demonKingPrefab,
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
        {
            challengeTracker.RewardIfSuccess();
            uiController.HideUI();
        }
    }

    private bool DoEnemiesRemain()
    {
        return spawnedEnemies.Count > 0;
    }
}
