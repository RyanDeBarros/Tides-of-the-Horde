using UnityEngine;
using UnityEngine.Assertions;

public class C_SurviveWithMinHP : IChallenge
{
    private Health health;
    private int hpThreshold = 0;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        health = player.GetComponent<Health>();
        Assert.IsNotNull(health);

        hpThreshold = Mathf.FloorToInt(Mathf.Min(values[0] * health.maxHealth, health.GetCurrentHealth()));
    }

    public bool IsSuccess()
    {
        return health.GetCurrentHealth() >= hpThreshold;
    }
}
