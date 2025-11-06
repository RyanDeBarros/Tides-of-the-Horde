using UnityEngine;
using UnityEngine.Assertions;

public class C_ShopDisabled : IChallenge
{
    private Health health;
    private ShopUI shop;

    public void Initialize(float[] values)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(player);
        health = player.GetComponent<Health>();
        Assert.IsNotNull(health);

        shop = GlobalFind.FindUniqueObjectByType<ShopUI>(true);
        Assert.IsNotNull(shop);

        shop.DisableShop();
    }

    public bool IsSuccess()
    {
        shop.EnableShop();
        return health.GetCurrentHealth() > 0;
    }
}
