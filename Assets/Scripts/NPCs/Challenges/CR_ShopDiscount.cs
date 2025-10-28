using UnityEngine;
using UnityEngine.Assertions;

public class CR_ShopDiscount : IReward
{
    private PlayerCurrency currency;
    private float discount = 0f;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        currency = player.GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);

        discount = values[0];
    }

    public void GiveReward()
    {
        currency.CompoundShopDiscount(discount);
    }
}
