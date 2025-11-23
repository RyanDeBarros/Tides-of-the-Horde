using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

class Portal : MonoBehaviour
{
    [SerializeField] private BoxCollider gateCollider;
    [SerializeField] private Transform playerSpawnPosition;
    [SerializeField] private float spawnDuration = 2f;
    [SerializeField] private float despawnDuration = 2f;
    public int levelIndex;

    [SerializeField] private GameObject portalVFX; // portal vfx only active when the portal is usable
    [SerializeField] private GameObject portalDespawnVFX; // vfx when the player despawns and goes to main menu
    [SerializeField] private AudioSource portalSFX;

    private PlayerCamera playerCamera;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private SpellManager spellManager;
    private Transform player;

    private Coroutine despawnRoutine = null;
    private bool despawnPlayer = false;

    private void Awake()
    {
        Assert.IsNotNull(gateCollider);
        Assert.IsNotNull(playerSpawnPosition);

        Assert.IsNotNull(portalVFX);
        Assert.IsNotNull(portalDespawnVFX);
        if (portalSFX == null)
            portalSFX = GetComponent<AudioSource>();
        Assert.IsNotNull(portalSFX);

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(go);
        player = go.transform;

        playerCamera = go.GetComponent<PlayerCamera>();
        Assert.IsNotNull(playerCamera);

        playerMovement = go.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);

        playerDash = go.GetComponent<PlayerDash>();
        Assert.IsNotNull(playerDash);

        spellManager = go.GetComponent<SpellManager>();
        Assert.IsNotNull(spellManager);

        LevelStatistics.Initialize();
    }

    private void Start()
    {
        portalSFX.Play();
    }

    public void Initialize(int levelIndex)
    {
        this.levelIndex = levelIndex;
    }

    private void Update()
    {
        if (despawnPlayer && despawnRoutine == null)
        {
            Collider[] cols = new Collider[1];
            if (Physics.OverlapBoxNonAlloc(
                    gateCollider.transform.TransformPoint(gateCollider.center),
                    0.5f * Vector3.Scale(gateCollider.transform.lossyScale, gateCollider.size),
                    cols,
                    gateCollider.transform.rotation,
                    gateCollider.includeLayers) > 0)
            {
                DespawnPlayer();
            }
        }
    }

    public void SpawnPlayer(Action playerSpawnedCallback)
    {
        portalVFX.SetActive(true);
        portalSFX.Play();
        portalDespawnVFX.SetActive(true);
        StartCoroutine(SpawnPlayerRoutine(playerSpawnedCallback));
    }

    private IEnumerator SpawnPlayerRoutine(Action playerSpawnedCallback)
    {
        SetPlayerEnable(false);
        player.SetPositionAndRotation(playerSpawnPosition.position, transform.rotation);
        yield return new WaitForSeconds(spawnDuration);
        portalVFX.SetActive(false);
        portalSFX.Stop();
        portalDespawnVFX.SetActive(false);
        SetPlayerEnable(true);
        playerSpawnedCallback();
    }

    public void PrepareToDespawnPlayer()
    {
        despawnPlayer = true;
        LevelStatistics.StopTimer();
        portalVFX.SetActive(true);
        portalSFX.Play();
    }

    private void DespawnPlayer()
    {
        despawnRoutine ??= StartCoroutine(EndLevelRoutine());
    }

    private IEnumerator EndLevelRoutine()
    {
        portalDespawnVFX.SetActive(true);
        SetPlayerEnable(false);

        Vector3 initialPos = player.position;
        for (float t = 0f; t < despawnDuration; t += Time.deltaTime)
        {
            player.position = Vector3.Lerp(initialPos, playerSpawnPosition.position, Mathf.Clamp01(t / despawnDuration));
            yield return null;
        }
        player.position = playerSpawnPosition.position;
        playerCamera.DisableCamera();
        LevelSelectUI.CompleteLevel(levelIndex);
    }

    private void SetPlayerEnable(bool enabled)
    {
        playerMovement.enabled = enabled;
        playerDash.enabled = enabled;
        spellManager.enabled = enabled;
        playerCamera.enabled = enabled;
    }

    public static Portal GetLevelPortal()
    {
        return GlobalFind.FindUniqueObjectByType<Portal>(true);
    }

    public static Transform GetPlayer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(go);
        return go.transform;
    }
}
