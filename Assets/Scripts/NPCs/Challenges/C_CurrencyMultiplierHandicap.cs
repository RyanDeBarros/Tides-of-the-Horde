using UnityEngine;
using UnityEngine.Assertions;

public class C_CurrencyMultiplierHandicap : IChallenge
{
    private Health health;
    private PlayerCurrency currency;
    private float prevMultiplier = 1f;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        health = player.GetComponent<Health>();
        Assert.IsNotNull(health);
        currency = player.GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);

        prevMultiplier = currency.GetMultiplier();
        currency.SetMultiplier(prevMultiplier * (1f - values[0]));
    }

    public bool IsSuccess()
    {
        currency.SetMultiplier(prevMultiplier);
        return health.GetCurrentHealth() > 0;
    }
}
