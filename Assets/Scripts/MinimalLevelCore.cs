using UnityEngine;
using UnityEngine.Assertions;

public class MinimalLevelCore : MonoBehaviour
{
    [SerializeField] private TextAsset spawnWaveFile;
    [SerializeField] private TextAsset dialogFile;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private NPCDialog npcDialog;

    private void Awake()
    {
        Assert.IsNotNull(enemySpawner);
        enemySpawner.Initialize(spawnWaveFile);

        Assert.IsNotNull(npcDialog);
        npcDialog.Initialize(dialogFile);
    }
}
