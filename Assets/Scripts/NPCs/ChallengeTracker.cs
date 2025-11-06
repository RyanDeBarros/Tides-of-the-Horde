using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class JSONChallengeList
{
    [Serializable]
    public class Challenge
    {
        public string statement;
        public string challengeClass;
        public float[] values;
        public int difficulty;
        public float weight = 1f;
    }

    [Serializable]
    public class Reward
    {
        public string statement;
        public string rewardClass;
        public float[] values;
        public int difficulty;
        public float weight = 1f;
    }

    [Serializable]
    public class Weight
    {
        public int difficulty;
        public float weight = 1f;
    }

    public List<Challenge> challenges;
    public List<Reward> rewards;
    public List<Weight> weights;
}

public interface IChallenge
{
    public void Initialize(float[] values);
    public bool IsSuccess();
}

public interface IReward
{
    public void Initialize(float[] values);
    public void GiveReward();
}

public class ChallengeTracker : MonoBehaviour
{
    [SerializeField] private TextAsset challengeFile;

    private class ChallengePerDifficulty
    {
        public float weight = 1f;
        public readonly List<JSONChallengeList.Challenge> challenges = new();
        public readonly List<JSONChallengeList.Reward> rewards = new();
    }

    private Dictionary<int, ChallengePerDifficulty> challengeDictionary = new();
    private JSONChallengeList.Challenge currentChallengeJSON = null;
    private JSONChallengeList.Reward currentRewardJSON = null;
    private IChallenge currentChallenge = null;
    private IReward currentReward = null;

    private void Awake()
    {
        Assert.IsNotNull(challengeFile);

        JSONChallengeList challengeList = JsonUtility.FromJson<JSONChallengeList>(challengeFile.text);
        challengeList.challenges.ForEach(challenge => {
            if (!challengeDictionary.ContainsKey(challenge.difficulty))
                challengeDictionary[challenge.difficulty] = new();
            challengeDictionary[challenge.difficulty].challenges.Add(challenge);
        });
        challengeList.rewards.ForEach(reward => {
            if (!challengeDictionary.ContainsKey(reward.difficulty))
                challengeDictionary[reward.difficulty] = new();
            challengeDictionary[reward.difficulty].rewards.Add(reward);
        });
        challengeList.weights.ForEach(weight => {
            if (!challengeDictionary.ContainsKey(weight.difficulty))
                challengeDictionary[weight.difficulty] = new();
            challengeDictionary[weight.difficulty].weight = weight.weight;
        });
        challengeDictionary = challengeDictionary.Where(kvp => kvp.Value.challenges.Count > 0 && kvp.Value.rewards.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.IsTrue(challengeDictionary.Count > 0);
    }

    public void SelectRandomChallenge()
    {
        currentChallenge = null;
        currentReward = null;

        ChallengePerDifficulty c = challengeDictionary[GetRandomDifficulty()];
        currentChallengeJSON = c.challenges.GetWeightedRandomElement(c.challenges.Select(challenge => challenge.weight).ToList());
        currentRewardJSON = c.rewards.GetWeightedRandomElement(c.rewards.Select(reward => reward.weight).ToList());
    }

    private int GetRandomDifficulty()
    {
        List<float> weights = challengeDictionary.Select(kvp => kvp.Value.weight).ToList();
        return challengeDictionary.Keys.ElementAt(weights.GetWeightedIndex());
    }

    public string GetChallengeStatement()
    {
        return currentChallengeJSON?.statement;
    }

    public string GetRewardStatement()
    {
        return currentRewardJSON?.statement;
    }

    public void AcceptChallenge()
    {
        if (currentChallengeJSON == null)
        {
            Debug.LogError("No challenge is set up.");
            return;
        }

        if (currentRewardJSON == null)
        {
            Debug.LogError("No reward is set up.");
            return;
        }

        currentChallenge = CreateInstanceFromJSON<IChallenge>(currentChallengeJSON.challengeClass);
        currentReward = CreateInstanceFromJSON<IReward>(currentRewardJSON.rewardClass);

        currentChallenge?.Initialize(currentChallengeJSON.values);
        currentReward?.Initialize(currentRewardJSON.values);

        // TODO display current challenge + reward in pause menu.
    }
    private T CreateInstanceFromJSON<T>(string className) where T : class
    {
        if (string.IsNullOrEmpty(className))
        {
            Debug.LogError($"No {typeof(T).Name} class name provided.");
            return null;
        }

        Type type = Type.GetType(className);
        if (type == null)
        {
            Debug.LogError($"{typeof(T).Name} class {className} not recognized.");
            return null;
        }

        object instance = Activator.CreateInstance(type);
        if (instance == null)
        {
            Debug.LogError($"Error occurred creating {className} instance.");
            return null;
        }

        if (instance is not T typedInstance)
        {
            Debug.LogError($"{className} does not implement {typeof(T).Name}.");
            return null;
        }

        return typedInstance;
    }

    public void DeclineChallenge()
    {
        currentChallengeJSON = null;
        currentRewardJSON = null;
        currentChallenge = null;
        currentReward = null;
    }

    public void RewardIfSuccess()
    {
        if (ChallengeCompleted())
            currentReward.GiveReward();
    }

    public bool HasChallenge()
    {
        return currentChallenge != null;
    }

    public bool ChallengeCompleted()
    {
        return HasChallenge() && currentChallenge.IsSuccess();
    }
}
