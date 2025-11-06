using UnityEngine;
using UnityEngine.Assertions;

public class MinimalLevelCore : MonoBehaviour
{
    [SerializeField] private TextAsset spawnWaveFile;
    [SerializeField] private TextAsset openingDialogFile;
    [SerializeField] private TextAsset closingDialogFile;
    [SerializeField] private int levelIndex;

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private NPCDialog npcDialog;
    [SerializeField] private Portal portal;

    private void Awake()
    {
        Assert.IsNotNull(enemySpawner);
        enemySpawner.Initialize(spawnWaveFile);

        Assert.IsNotNull(npcDialog);
        npcDialog.Initialize(openingDialogFile, closingDialogFile);

        Assert.IsNotNull(portal);
        portal.Initialize(levelIndex);
    }
}
