using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentChallengeData
{
    public class ChallengeData
    {
        private int _startingBonus = 0;
        public int StartingBonus
        {
            get => _startingBonus;
            set => SetField(ref _startingBonus, value);
        }

        private float _shopDiscount = 0f;
        public float ShopDiscount
        {
            get => _shopDiscount;
            set => SetField(ref _shopDiscount, value);
        }

        private float _currencyBoost = 0f;
        public float CurrencyBoost
        {
            get => _currencyBoost;
            set => SetField(ref _currencyBoost, value);
        }

        private Dictionary<int, HashSet<string>> _completedRewardsPerLevel = new();
        public Dictionary<int, HashSet<string>> CompletedRewardsPerLevel
        {
            get => _completedRewardsPerLevel;
            set => SetField(ref _completedRewardsPerLevel, value);
        }

        private HashSet<string> GetCompletedRewards(int level)
        {
            if (!CompletedRewardsPerLevel.ContainsKey(level))
                CompletedRewardsPerLevel[level] = new();
            return CompletedRewardsPerLevel[level];
        }

        public bool IsRewardCompleted(int level, string reward)
        {
            return GetCompletedRewards(level).Contains(reward);
        }

        public void CompleteReward(int level, string reward)
        {
            GetCompletedRewards(level).Add(reward);
            Save();
        }

        [JsonIgnore]
        public bool SaveOnSet { get; set; }

        private void SetField<T>(ref T field, T value)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                if (SaveOnSet)
                    Save();
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
            data.SaveOnSet = true;
        }
        return data;
    }

    public static void Save()
    {
        PlayerDataManager.Set(KEY, Data());
    }

    public static void Reset()
    {
        data = new() { SaveOnSet = true };
    }
}
