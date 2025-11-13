public class CR_CurrencyStartingBonus : IReward
{
    private int bonus = 0;

    public void Initialize(float[] values)
    {
        bonus = (int)values[0];
    }

    public void GiveReward()
    {
        PersistentChallengeData.Data().StartingBonus += bonus;
    }
}
