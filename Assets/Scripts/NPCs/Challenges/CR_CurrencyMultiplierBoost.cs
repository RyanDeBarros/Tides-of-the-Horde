using UnityEngine;
using UnityEngine.Assertions;

public class CR_CurrencyMultiplierBoost : IReward
{
    private PlayerCurrency currency;
    private float boost = 0f;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        currency = player.GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);

        boost = values[0];
    }

    public void GiveReward()
    {
        currency.AddMultiplier(boost);
    }
}
