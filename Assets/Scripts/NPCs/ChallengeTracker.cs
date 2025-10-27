using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class JSONChallenge
{
    public string statement;
    public string challengeClass;
    public float[] values;
    public int difficulty;
}

[Serializable]
public class JSONReward
{
    public string statement;
    public string rewardClass;
    public float[] values;
    public int difficulty;
}

[Serializable]
public class JSONChallengeList
{
    public List<JSONChallenge> challenges;
    public List<JSONReward> rewards;
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
        public readonly List<JSONChallenge> challenges = new();
        public readonly List<JSONReward> rewards = new();
    }

    private Dictionary<int, ChallengePerDifficulty> challengeDictionary = new();
    private JSONChallenge currentChallengeJSON = null;
    private JSONReward currentRewardJSON = null;
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
        challengeDictionary = challengeDictionary.Where(kvp => kvp.Value.challenges.Count > 0 && kvp.Value.rewards.Count > 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Assert.IsTrue(challengeDictionary.Count > 0);
    }

    public void SelectRandomChallenge()
    {
        currentChallenge = null;
        currentReward = null;

        int difficulty = 1; // TODO choose difficulty
        ChallengePerDifficulty c = challengeDictionary[difficulty];
        currentChallengeJSON = c.challenges[UnityEngine.Random.Range(0, c.challenges.Count - 1)];
        currentRewardJSON = c.rewards[UnityEngine.Random.Range(0, c.rewards.Count - 1)];
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
    }

    public void RewardIfSuccess()
    {
        if (currentChallenge.IsSuccess())
            currentReward.GiveReward();
    }
}
