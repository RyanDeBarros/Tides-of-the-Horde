using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {
        // DemonKingMovementAI
        public float moveSpeed = 5f;
        public float chaseRange = 10f;

        public float sinkSpeed = 3f; // Speed at which boss sinks into ground
        public float sinkDepth = 5f; // How far underground to go
        public float teleportDuration = 2f; // Total time underground
        public float behindPlayerDistance = 3f; // Distance behind player to spawn

        // SwordHitbox
        public int damage = 5;
        public float bounceBackStrength = 75f;

        // TargetDetector
        public float attackRange = 7f;
        public float attackInterval = 1f;

        // Health
        public int maxHealth = 1000;

        // BounceBack
        public float bounceBackResistance = 2f;
        public float bounceBackDuration = 0.07f;

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

    // TODO move from Start() to SetDifficultyLevel() - add GetDifficultyLevel()
    private void Start()
    {
        DifficultyStats stats = difficultyStatsList.stats[System.Math.Clamp(difficultyLevel - 1, 0, difficultyStatsList.stats.Count - 1)];

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

    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;
    }
}
