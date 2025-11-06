using UnityEngine;
using UnityEngine.Assertions;

public class CR_CurrencyStartingBonus : IReward
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
        currency.Add(bonus); // TODO should be given at the beginning of level. Use a singleton pattern for PlayerData that keeps track of obtained rewards. Serialize it during save/load
    }
}
