using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SkeletonDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {
        // SkeletonMovementAI
        public float moveSpeed = 5f;
        public float chaseRange = 30f;

        // TargetDetector
        public float attackRange = 5f;
        public float attackInterval = 1f;

        // Health
        public int maxHealth = 100;

        // BounceBack
        public float bounceBackResistance = 1f;
        public float bounceBackDuration = 0.1f;

        // MeleeHitbox
        public int damage = 1;

        // RewardOnDeath
        public int reward = 10;
    }

    [System.Serializable]
    public class DifficultyStatsList
    {
        public List<DifficultyStats> stats;
    }

    [SerializeField] private TextAsset statsFile;

    private static DifficultyStatsList difficultyStatsList = null;

    private SkeletonMovementAI movement;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private MeleeHitbox melee;
    private RewardOnDeath reward;

    private int difficultyLevel;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<SkeletonMovementAI>();
        Assert.IsNotNull(movement);
        detector = GetComponent<TargetDetector>();
        Assert.IsNotNull(detector);
        health = GetComponent<Health>();
        Assert.IsNotNull(health);
        bounceBack = GetComponent<BounceBack>();
        Assert.IsNotNull(bounceBack);
        melee = GetComponentInChildren<MeleeHitbox>();
        Assert.IsNotNull(melee);
        reward = GetComponent<RewardOnDeath>();
        Assert.IsNotNull(reward);
    }

    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;

        DifficultyStats stats = difficultyStatsList.stats[System.Math.Clamp(difficultyLevel - 1, 0, difficultyStatsList.stats.Count - 1)];

        movement.moveSpeed = stats.moveSpeed;
        movement.chaseRange = stats.chaseRange;

        detector.attackRange = stats.attackRange;
        detector.attackInterval = stats.attackInterval;

        health.maxHealth = stats.maxHealth;

        bounceBack.resistance = stats.bounceBackResistance;
        bounceBack.duration = stats.bounceBackDuration;

        melee.damage = stats.damage;

        reward.reward = stats.reward;
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }
}
