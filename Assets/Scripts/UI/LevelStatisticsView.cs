using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelStatisticsView : MonoBehaviour
{
    [SerializeField] private string song = "Level Statistics";

    [Header("Main Stats")]
    [SerializeField] private TextMeshProUGUI finalHealthText;
    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI totalTimeText;

    [Header("Enemies Defeated")]
    [SerializeField] private TextMeshProUGUI skeletonsText;
    [SerializeField] private TextMeshProUGUI orcsText;
    [SerializeField] private TextMeshProUGUI bishopsText;
    [SerializeField] private TextMeshProUGUI dragonsText;

    [Header("Bosses Defeated")]
    [SerializeField] private TextMeshProUGUI flyingDemonsText;
    [SerializeField] private TextMeshProUGUI demonKingsText;

    private void Awake()
    {
        Assert.IsNotNull(finalHealthText);
        Assert.IsNotNull(totalCurrencyText);
        Assert.IsNotNull(totalTimeText);

        Assert.IsNotNull(skeletonsText);
        Assert.IsNotNull(orcsText);
        Assert.IsNotNull(bishopsText);
        Assert.IsNotNull(dragonsText);

        Assert.IsNotNull(flyingDemonsText);
        Assert.IsNotNull(demonKingsText);
    }

    private void Start()
    {
        SoundtrackManager.Instance.PlayTrack(song);

        finalHealthText.SetText($"{LevelStatistics.GetFinalHealth()}/{LevelStatistics.GetMaxHealth()}");
        totalCurrencyText.SetText($"{LevelStatistics.GetTotalCurrency()}");
        int totalTime = (int)LevelStatistics.GetTotalTime();
        totalTimeText.SetText($"{(totalTime / 60)}:{totalTime % 60 :D2}");

        static void SetNumberOfEnemies(EnemyType enemyType, TextMeshProUGUI enemyText)
        {
            int enemies = LevelStatistics.GetEnemiesDefeated(enemyType);
            enemyText.SetText($"{enemies}");
            enemyText.transform.parent.gameObject.SetActive(enemies > 0);
        }

        SetNumberOfEnemies(EnemyType.Skeleton, skeletonsText);
        SetNumberOfEnemies(EnemyType.Orc, orcsText);
        SetNumberOfEnemies(EnemyType.Bishop, bishopsText);
        SetNumberOfEnemies(EnemyType.Dragon, dragonsText);
        SetNumberOfEnemies(EnemyType.FlyingDemon, flyingDemonsText);
        SetNumberOfEnemies(EnemyType.DemonKing, demonKingsText);
    }

    public void OnContinue()
    {
        SceneSwitcher.OpenLevelSelect();
    }
}
