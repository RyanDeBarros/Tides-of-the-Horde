using UnityEngine;

public class CR_ShopDiscount : IReward
{
    private float discount = 0f;

    public void Initialize(float[] values)
    {
        discount = Mathf.Clamp01(values[0]);
    }

    public void GiveReward()
    {
        PersistentChallengeData.Data().ShopDiscount += discount * (1f - PersistentChallengeData.Data().ShopDiscount);
    }
}
