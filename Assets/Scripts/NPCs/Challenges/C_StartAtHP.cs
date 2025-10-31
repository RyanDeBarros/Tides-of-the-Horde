using UnityEngine;
using UnityEngine.Assertions;

public class C_StartAtHP : IChallenge
{
    private Health health;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        health = player.GetComponent<Health>();
        Assert.IsNotNull(health);

        health.LowerToPercent(values[0]);
    }

    public bool IsSuccess()
    {
        return health.GetCurrentHealth() > 0;
    }
}
