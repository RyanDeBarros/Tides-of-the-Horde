using UnityEngine;
using UnityEngine.Assertions;

public class MinimalLevelCore : MonoBehaviour
{
    [SerializeField] private TextAsset spawnWaveFile;
    [SerializeField] private TextAsset dialogFile;

    private void Awake()
    {
        EnemySpawner enemySpawner = GetComponentInChildren<EnemySpawner>();
        Assert.IsNotNull(enemySpawner);
        enemySpawner.Initialize(spawnWaveFile);

        NPCDialog dialog = GetComponentInChildren<NPCDialog>();
        Assert.IsNotNull(dialog);
        dialog.Initialize(dialogFile);
    }
}
