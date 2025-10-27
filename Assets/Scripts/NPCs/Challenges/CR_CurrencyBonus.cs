using UnityEngine;
using UnityEngine.Assertions;

public class CR_CurrencyBonus : IReward
{
    private PlayerCurrency currency;
    private int bonus = 0;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        currency = player.GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);

        bonus = (int)values[0];
    }

    public void GiveReward()
    {
        currency.Add(bonus);
    }
}
