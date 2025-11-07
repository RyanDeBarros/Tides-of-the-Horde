using Newtonsoft.Json;
using UnityEngine;

public static class PersistentChallengeData
{
    // TODO rewards should not be claimed multiple times. Once a reward is claimed, it cannot show up again. Handle edge case where no rewards are left. Also, make rewards level specific so that challenges are spread out over multiple levels.
    public class ChallengeData
    {
        private int _startingBonus = 0;
        private float _shopDiscount = 0f;
        private float _currencyBoost = 0f;

        [JsonIgnore]
        public bool saveOnSet = false;

        public int StartingBonus
        {
            get => _startingBonus;
            set
            {
                if (_startingBonus != value)
                {
                    _startingBonus = value;
                    if (saveOnSet)
                        PersistentChallengeData.Save();
                }
            }
        }

        public float ShopDiscount
        {
            get => _shopDiscount;
            set
            {
                if (Mathf.Abs(_shopDiscount - value) > 0.0001f)
                {
                    _shopDiscount = value;
                    if (saveOnSet)
                        PersistentChallengeData.Save();
                }
            }
        }

        public float CurrencyBoost
        {
            get => _currencyBoost;
            set
            {
                if (Mathf.Abs(_currencyBoost - value) > 0.0001f)
                {
                    _currencyBoost = value;
                    if (saveOnSet)
                        PersistentChallengeData.Save();
                }
            }
        }
    }

    private static ChallengeData data = null;
    private const string KEY = "CHALLENGE_DATA";

    public static ChallengeData Data()
    {
        if (data == null)
        {
            data = new();
            PlayerDataManager.Get(KEY, ref data);
            data.saveOnSet = true;
        }
        return data;
    }

    public static void Save()
    {
        PlayerDataManager.Set(KEY, Data());
    }
}
