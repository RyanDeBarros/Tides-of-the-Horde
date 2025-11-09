using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelStatisticsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI finalHealthText;
    [SerializeField] private TextMeshProUGUI totalCurrencyText;
    [SerializeField] private TextMeshProUGUI totalTimeText;

    private void Awake()
    {
        Assert.IsNotNull(finalHealthText);
        Assert.IsNotNull(totalCurrencyText);
        Assert.IsNotNull(totalTimeText);
    }

    private void Start()
    {
        finalHealthText.SetText($"{LevelStatistics.GetFinalHealth()}/{LevelStatistics.GetMaxHealth()}");
        totalCurrencyText.SetText($"{LevelStatistics.GetTotalCurrency()}");
        int totalTime = (int)LevelStatistics.GetTotalTime();
        totalTimeText.SetText($"{(totalTime / 60)}:{totalTime % 60 :D2}");

        // TODO display stastics in UI
        Debug.Log($"# skeletons defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.Skeleton)}");
        Debug.Log($"# orcs defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.Orc)}");
        Debug.Log($"# bishops defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.Bishop)}");
        Debug.Log($"# dragons defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.Dragon)}");
        Debug.Log($"# flying demons defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.FlyingDemon)}");
        Debug.Log($"# demon kings defeated = {LevelStatistics.GetEnemiesDefeated(EnemyType.DemonKing)}");
    }

    public void OnContinue()
    {
        SceneSwitcher.OpenLevelSelect();
    }
}
