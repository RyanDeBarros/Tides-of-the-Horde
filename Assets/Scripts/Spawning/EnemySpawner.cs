using System;
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
    [SerializeField] private Portal portal;
    [SerializeField] private string waitForChallengerSong = "Peaceful";
    [SerializeField] private string wavesCompletedSong = "Peaceful";

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject bishopPrefab;
    [SerializeField] private GameObject orcPrefab;
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private GameObject flyingDemonPrefab;
    [SerializeField] private GameObject demonKingPrefab;

    [Min(0)] public int difficultyLevelOffset = 0;

    private WaveTimeline waveTimeline;
    private List<SpawnZone> spawnZones;
    private List<Waypoint> waypoints;
    private readonly HashSet<GameObject> spawnedEnemies = new();

    private enum LevelPhase
    {
        PortalStart,
        ChallengeGiverStart,
        Waves,
        ChallengeGiverEnd,
        PortalEnd
    }

    private LevelPhase levelPhase = LevelPhase.PortalStart;

    public void Initialize(TextAsset waveFile)
    {
        this.waveFile = waveFile;
    }

    private void Awake()
    {
        Assert.IsNotNull(uiController);
        Assert.IsNotNull(shopUI);
        Assert.IsNotNull(challengeGiver);
        challengeGiver.onConversationEnd.AddListener(OnConversationEnd);
        Assert.IsNotNull(challengeTracker);
        Assert.IsNotNull(portal);

        Assert.IsNotNull(skeletonPrefab);
        Assert.IsNotNull(bishopPrefab);
        Assert.IsNotNull(orcPrefab);
        Assert.IsNotNull(demonKingPrefab);

        spawnZones = new(FindObjectsByType<SpawnZone>(FindObjectsSortMode.None));
        waypoints = new(FindObjectsByType<Waypoint>(FindObjectsSortMode.InstanceID));
        waypoints.Sort((w1, w2) => w1.index - w2.index);
    }

    private void Start()
    {
        Assert.IsNotNull(waveFile);
        waveTimeline = WaveTimeline.Read(waveFile);

        uiController.HideUI();
        shopUI.gameObject.SetActive(false);

        SoundtrackManager.Instance.PlayTrack(waitForChallengerSong);
        portal.SpawnPlayer(() => {
            levelPhase = LevelPhase.ChallengeGiverStart;
            challengeGiver.SpawnNPC();
        });
    }

    private void Update()
    {
        if (levelPhase == LevelPhase.Waves)
            UpdateSpawnTimeline();
    }

    private void OnConversationEnd()
    {
        if (levelPhase == LevelPhase.ChallengeGiverStart)
            StartWaves();
        else if (levelPhase == LevelPhase.ChallengeGiverEnd)
            portal.PrepareToDespawnPlayer();
        else
            throw new NotImplementedException();
    }

    private void StartWaves()
    {
        uiController.ShowUI();
        shopUI.gameObject.SetActive(true);
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

        foreach (((EnemyType type, int difficultyLevel, bool isBoss), int toSpawn) in waveTimeline.GetEnemiesToSpawn())
            SpawnEnemies(type, toSpawn, difficultyLevel, isBoss);
    }

    public void SpawnEnemies(EnemyType type, int numEnemies, int difficultyLevel, bool isBoss)
    {
        if (numEnemies <= 0) return;
        List<SpawnZone> activeSpawnZones = spawnZones.Where(spawner => spawner.IsSpawnable()).ToList();
        if (activeSpawnZones.Count == 0) return;

        for (int _ = 0; _ < numEnemies; ++_)
        {
            Vector3 spawnPoint = activeSpawnZones.GetRandomElement().GetRandomPoint();
            SpawnAtPoint(type, spawnPoint, difficultyLevel, isBoss);
        }
    }

    private void SpawnAtPoint(EnemyType type, Vector3 point, int difficultyLevel, bool isBoss)
    {
        GameObject instance = Instantiate(GetEnemyPrefab(type), point, Quaternion.identity);
        spawnedEnemies.Add(instance);
        instance.AddComponent<OnDestroyHandler>().onDestroyed.AddListener(go => spawnedEnemies.Remove(go));
        if (instance.TryGetComponent(out IDifficultyImplementer difficulty))
            difficulty.SetDifficultyLevel(difficultyLevel + difficultyLevelOffset);
        if (instance.TryGetComponent(out WaypointPatroller waypointPatroller))
            waypointPatroller.waypoints = waypoints;
        if (instance.TryGetComponent(out Health health))
            health.onDeath.AddListener(() => { LevelStatistics.AddEnemyDeath(type); });
        if (instance.TryGetComponent(out BossHealthBarSelector selector))
            selector.SetBoss(isBoss);
    }

    private GameObject GetEnemyPrefab(EnemyType type)
    {
        GameObject prefab = type switch
        {
            EnemyType.Skeleton => skeletonPrefab,
            EnemyType.Bishop => bishopPrefab,
            EnemyType.Orc => orcPrefab,
            EnemyType.Dragon => dragonPrefab,
            EnemyType.FlyingDemon => flyingDemonPrefab,
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
            uiController.HideUI();
            levelPhase = LevelPhase.ChallengeGiverEnd;
            SoundtrackManager.Instance.PlayTrack(wavesCompletedSong);
            challengeGiver.GetDialog().dialogPhase = NPCDialog.DialogPhase.Closing;
            challengeGiver.SpawnNPC();
        }
    }

    private bool DoEnemiesRemain()
    {
        return spawnedEnemies.Count > 0;
    }
}
