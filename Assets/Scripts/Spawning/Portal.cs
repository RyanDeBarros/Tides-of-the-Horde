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
    [SerializeField] private int levelIndex;

    private PlayerCamera playerCamera;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private SpellManager spellManager;
    private Transform player;
    private Transform playerModel;

    private bool despawnPlayer = false;

    private void Awake()
    {
        Assert.IsNotNull(gateCollider);
        Assert.IsNotNull(playerSpawnPosition);

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

        var model = go.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(model);
        playerModel = model.transform;
    }

    public void Initialize(int levelIndex)
    {
        this.levelIndex = levelIndex;
    }

    private void Update()
    {
        if (despawnPlayer)
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
        StartCoroutine(SpawnPlayerRoutine(playerSpawnedCallback));
    }

    private IEnumerator SpawnPlayerRoutine(Action playerSpawnedCallback)
    {
        SetPlayerEnable(false);

        player.SetPositionAndRotation(playerSpawnPosition.position, transform.rotation);
        playerModel.localScale = new (1f, 0f, 1f);

        for (float t = 0f; t < spawnDuration; t += Time.deltaTime)
        {
            playerModel.localScale = new(1f, t / spawnDuration, 1f);
            yield return null;
        }

        playerModel.localScale = Vector3.one;
        SetPlayerEnable(true);
        playerSpawnedCallback();
    }

    public void PrepareToDespawnPlayer()
    {
        despawnPlayer = true;
        // TODO VFX to show that player can enter portal
    }

    private void DespawnPlayer()
    {
        StartCoroutine(EndLevelRoutine());
    }

    private IEnumerator EndLevelRoutine()
    {
        SetPlayerEnable(false);

        playerModel.localScale = Vector3.one;

        for (float t = 0f; t < despawnDuration; t += Time.deltaTime)
        {
            playerModel.localScale = new(1f, 1f - t / despawnDuration, 1f);
            yield return null;
        }

        playerModel.localScale = new(1f, 0f, 1f);

        playerCamera.DisableCamera();
        LevelSelectUI.CompleteLevel(levelIndex);
    }

    private void SetPlayerEnable(bool enabled)
    {
        playerMovement.enabled = enabled;
        playerDash.enabled = enabled;
        spellManager.enabled = enabled;
    }
}
