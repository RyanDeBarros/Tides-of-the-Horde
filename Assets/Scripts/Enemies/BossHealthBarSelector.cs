using UnityEngine;
using UnityEngine.Assertions;

public class BossHealthBarSelector : MonoBehaviour
{
    [SerializeField] private EnemyHealthBar enemyHealthBar;
    [SerializeField] private BossHealthBarTarget bossHealthBar;

    private void Awake()
    {
        if (enemyHealthBar == null)
            enemyHealthBar = GetComponent<EnemyHealthBar>();
        Assert.IsNotNull(enemyHealthBar);

        if (bossHealthBar == null)
            bossHealthBar = GetComponent<BossHealthBarTarget>();
        Assert.IsNotNull(bossHealthBar);
    }

    public void SetBoss(bool isBoss)
    {
        enemyHealthBar.enabled = !isBoss;
        bossHealthBar.enabled = isBoss;
    }
}
