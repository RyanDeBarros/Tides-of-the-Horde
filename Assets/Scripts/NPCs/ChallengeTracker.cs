using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
class LevelChallengeList
{
    [Serializable]
    public class Challenge
    {
        public string statement;
        public string challengeClass;
        public float[] values;
        public float weight = 1f;
    }

    [Serializable]
    public class Reward
    {
        public string statement;
        public string rewardClass;
        public float[] values;
        public float weight = 1f;
    }

    public List<Challenge> challenges;
    public List<Reward> rewards;
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

    private Dictionary<int, LevelChallengeList> challengeMap = new();

    private LevelChallengeList.Challenge currentChallengeJSON = null;
    private LevelChallengeList.Reward currentRewardJSON = null;
    private IChallenge currentChallenge = null;
    private IReward currentReward = null;

    private void Awake()
    {
        Assert.IsNotNull(challengeFile);
        challengeMap = JsonConvert.DeserializeObject<Dictionary<int, LevelChallengeList>>(challengeFile.text);
    }

    public bool SelectRandomChallenge()
    {
        currentChallenge = null;
        currentReward = null;

        int levelIndex = Portal.GetLevelPortal().levelIndex;
        var rewards = challengeMap[levelIndex].rewards.Where(reward => !PersistentChallengeData.Data().IsRewardCompleted(levelIndex, reward.rewardClass));
        if (rewards.Any())
        {
            currentRewardJSON = rewards.ToList().GetWeightedRandomElement(rewards.Select(reward => reward.weight).ToList());
            List<LevelChallengeList.Challenge> challenges = challengeMap[levelIndex].challenges;
            currentChallengeJSON = challenges.GetWeightedRandomElement(challenges.Select(challenge => challenge.weight).ToList());
            return true;
        }
        else
            return false;
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
        currentRewardJSON = null;
        currentChallenge = null;
        currentReward = null;
    }

    public void GiveReward()
    {
        currentReward.GiveReward();
        PersistentChallengeData.Data().CompleteReward(Portal.GetLevelPortal().levelIndex, currentReward.GetType().Name);
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
