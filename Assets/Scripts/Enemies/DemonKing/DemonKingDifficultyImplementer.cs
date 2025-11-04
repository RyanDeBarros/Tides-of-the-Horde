using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {
        // DemonKingMovementAI
        public float moveSpeed = 10f;
        public float chaseRange = 30f;

        public float sinkSpeed = 6f; // Speed at which boss sinks into ground
        public float sinkDepth = 5f; // How far underground to go
        public float teleportDuration = 3f; // Total time underground
        public float behindPlayerDistance = 4f; // Distance behind player to spawn

        // SwordHitbox
        public int damage = 5;
        public float bounceBackStrength = 75f;

        // TargetDetector
        public float attackRange = 7f;
        public float attackInterval = 1f;

        // Health
        public int maxHealth = 3000;

        // BounceBack
        public float bounceBackResistance = 2f;
        public float bounceBackDuration = 0.07f;

        // RewardOnDeath
        public int reward = 100;
    }

    [System.Serializable]
    public class DifficultyStatsList
    {
        public List<DifficultyStats> stats;
    }

    [SerializeField] private TextAsset statsFile;

    private static DifficultyStatsList difficultyStatsList = null;

    private DemonKingMovementAI movement;
    private SwordHitbox melee;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);
        melee = GetComponentInChildren<SwordHitbox>();
        Assert.IsNotNull(melee);
        detector = GetComponent<TargetDetector>();
        Assert.IsNotNull(detector);
        health = GetComponent<Health>();
        Assert.IsNotNull(health);
        bounceBack = GetComponent<BounceBack>();
        Assert.IsNotNull(bounceBack);
        reward = GetComponent<RewardOnDeath>();
        Assert.IsNotNull(reward);
    }

    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = System.Math.Clamp(level, 1, difficultyStatsList.stats.Count);
        DifficultyStats stats = difficultyStatsList.stats[difficultyLevel - 1];

        movement.moveSpeed = stats.moveSpeed;
        movement.chaseRange = stats.chaseRange;
        movement.sinkSpeed = stats.sinkSpeed;
        movement.sinkDepth = stats.sinkDepth;
        movement.teleportDuration = stats.teleportDuration;
        movement.behindPlayerDistance = stats.behindPlayerDistance;

        melee.damage = stats.damage;
        melee.bounceBackStrength = stats.bounceBackStrength;

        detector.attackRange = stats.attackRange;
        detector.attackInterval = stats.attackInterval;

        health.maxHealth = stats.maxHealth;

        bounceBack.resistance = stats.bounceBackResistance;
        bounceBack.duration = stats.bounceBackDuration;

        reward.reward = stats.reward;
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }

    public void GetSmarter()
    {
        // TODO use lists in stats file for these values:
        // TODO increase damage
        // TODO increase speed
        // TODO increase teleport frequency
    }
}
