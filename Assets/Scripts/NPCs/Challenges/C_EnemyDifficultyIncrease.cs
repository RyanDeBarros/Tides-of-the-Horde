using UnityEngine;
using UnityEngine.Assertions;

public class C_EnemyDifficultyIncrease : IChallenge
{
    private EnemySpawner spawner;
    private Health health;

    public void Initialize(float[] values)
    {
        spawner = GlobalFind.FindUniqueObjectByType<EnemySpawner>(true);
        Assert.IsNotNull(spawner);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        health = player.GetComponent<Health>();
        Assert.IsNotNull(health);

        spawner.difficultyLevelOffset = System.Math.Max((int)values[0], 0);
    }

    public bool IsSuccess()
    {
        spawner.difficultyLevelOffset = 0;
        return health.GetCurrentHealth() > 0;
    }
}
