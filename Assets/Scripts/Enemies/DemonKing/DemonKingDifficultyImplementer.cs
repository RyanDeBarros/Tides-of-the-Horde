using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {
        // DemonKingMovementAI
        public float moveSpeed = 10f;
        public float chaseRange = 60f;
        public float turnSpeed = 800f;

        public float sinkSpeed = 6f; // Speed at which boss sinks into ground
        public float riseSpeed = 8f; // Speed at which boss rises from ground
        public float sinkDepth = 5f; // How far underground to go
        public float teleportDuration = 2f; // Total time underground
        public float behindPlayerDistance = 4f; // Distance behind player to spawn
        public List<float> minRegularTeleportDelay = new() { 4f, 3.7f, 3.4f, 3.1f, 2.8f };
        public List<float> maxRegularTeleportDelay = new() { 20f, 19f, 18f, 17f, 16f };

        // DemonKingAttackAI
        public List<float> comboProbability = new() { 0.1f, 0.15f, 0.2f, 0.25f, 0.3f };

        public List<float> minRangedAttackDelay = new() { 4f, 3.7f, 3.4f, 3.1f, 2.8f };
        public List<float> maxRangedAttackDelay = new() { 20f, 19f, 18f, 17f, 16f };

        public int spikesInitialDamage = 5;
        public float spikesDamageOverTime = 5f;
        public float spikesSlowingFactor = 0.75f;
        public float spikesFocusRadius = 20f;
        public float spikesTelegraphDuration = 2f;
        public float spikesRisingDuration = 0.5f;
        public float spikesStayingDuration = 0.5f;
        public float spikesFallingDuration = 0.2f;
        public List<int> spikeTrapCount = new() { 4, 5, 6, 7, 8 };

        // SwordHitboxController
        public List<int> damage = new() { 5, 6, 7, 8, 9 };
        public float bounceBackStrength = 75f;

        // TargetDetector
        public List<float> attackRange = new() { 7f, 7.25f, 7.5f, 7.75f, 8f };
        public List<float> attackInterval = new() { 1.7f, 1.65f, 0.6f, 1.55f, 1.5f };

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
    private DemonKingAttackAI attackAI;
    private SwordHitboxController melee;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel = 1;
    private int intelligence = 0;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);
        attackAI = GetComponent<DemonKingAttackAI>();
        Assert.IsNotNull(attackAI);
        melee = GetComponentInChildren<SwordHitboxController>();
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
        difficultyLevel = Math.Clamp(level, 1, difficultyStatsList.stats.Count);
        DifficultyStats stats = difficultyStatsList.stats[difficultyLevel - 1];

        movement.moveSpeed = stats.moveSpeed;
        movement.chaseRange = stats.chaseRange;
        movement.turnSpeed = stats.turnSpeed;
        movement.sinkSpeed = stats.sinkSpeed;
        movement.riseSpeed = stats.riseSpeed;
        movement.sinkDepth = stats.sinkDepth;
        movement.teleportDuration = stats.teleportDuration;
        movement.behindPlayerDistance = stats.behindPlayerDistance;
        movement.minRegularTeleportDelay = stats.minRegularTeleportDelay[intelligence];
        movement.maxRegularTeleportDelay = stats.maxRegularTeleportDelay[intelligence];
        
        attackAI.comboProbability = stats.comboProbability[intelligence];
        attackAI.minRangedAttackDelay = stats.minRangedAttackDelay[intelligence];
        attackAI.maxRangedAttackDelay = stats.maxRangedAttackDelay[intelligence];
        
        attackAI.spikesInitialDamage = stats.spikesInitialDamage;
        attackAI.spikesDamageOverTime = stats.spikesDamageOverTime;
        attackAI.spikesSlowingFactor = stats.spikesSlowingFactor;
        attackAI.spikesFocusRadius = stats.spikesFocusRadius;
        attackAI.spikesTelegraphDuration = stats.spikesTelegraphDuration;
        attackAI.spikesRisingDuration = stats.spikesRisingDuration;
        attackAI.spikesStayingDuration = stats.spikesStayingDuration;
        attackAI.spikesFallingDuration = stats.spikesFallingDuration;
        attackAI.spikeTrapCount = stats.spikeTrapCount[intelligence];

        melee.damage = stats.damage[intelligence];
        melee.bounceBackStrength = stats.bounceBackStrength;

        detector.attackRange = stats.attackRange[intelligence];
        detector.attackInterval = stats.attackInterval[intelligence];

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
        DifficultyStats stats = difficultyStatsList.stats[difficultyLevel - 1];
        ++intelligence;

        stats.minRegularTeleportDelay.AssignToIfInRange(intelligence, ref movement.minRegularTeleportDelay);
        stats.maxRegularTeleportDelay.AssignToIfInRange(intelligence, ref movement.maxRegularTeleportDelay);
        stats.minRangedAttackDelay.AssignToIfInRange(intelligence, ref attackAI.minRangedAttackDelay);
        stats.maxRangedAttackDelay.AssignToIfInRange(intelligence, ref attackAI.maxRangedAttackDelay);
        stats.spikeTrapCount.AssignToIfInRange(intelligence, ref attackAI.spikeTrapCount);
        stats.damage.AssignToIfInRange(intelligence, ref melee.damage);
        stats.attackRange.AssignToIfInRange(intelligence, ref detector.attackRange);
        stats.attackInterval.AssignToIfInRange(intelligence, ref detector.attackInterval);
    }
}
