using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossShield : MonoBehaviour
{
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private Health bossHealth;

    // TODO add to DemonKingDifficultyImplementer
    public int keysRequired = 3;
    public float keySpawnCooldown = 10f;
    private float timeElapsed = 0f;

    private List<KeySpawnZone> spawnZones;

    private void Awake()
    {
        Assert.IsNotNull(keyPrefab);
        Assert.IsNotNull(keyPrefab.GetComponent<Key>());

        if (bossHealth == null)
            bossHealth = GetComponent<Health>();
        Assert.IsNotNull(bossHealth);

        spawnZones = new(FindObjectsByType<KeySpawnZone>(FindObjectsSortMode.None));
        Assert.IsTrue(spawnZones.Count > 0);
    }

    private void Start()
    {
        bossHealth.SetInvulnerable(true);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        while (timeElapsed >= keySpawnCooldown)
        {
            SpawnKey();
            timeElapsed -= keySpawnCooldown;
        }
    }

    public void SpawnKey()
    {
        SpawnKey(spawnZones.GetRandomElement().GetRandomPoint());
    }

    public void SpawnKey(Vector3 position)
    {
        GameObject go = Instantiate(keyPrefab, position, Quaternion.identity);
        Assert.IsNotNull(go);
        if (go.TryGetComponent(out Key key))
            key.shield = this;
        else
            Destroy(go);
    }

    public void CollectKey()
    {
        --keysRequired;
        if (keysRequired <= 0)
        {
            bossHealth.SetInvulnerable(false);
            enabled = false;
        }
    }
}
