public class CR_CurrencyMultiplierBoost : IReward
{
    private float boost = 0f;

    public void Initialize(float[] values)
    {
        boost = values[0] > 0f ? values[0] : 0f;
    }

    public void GiveReward()
    {
        PersistentChallengeData.Data().CurrencyBoost += boost;
    }
}
