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
        public float chaseRange = 30f;
        public float turnSpeed = 800f;

        public float sinkSpeed = 6f; // Speed at which boss sinks into ground
        public float sinkDepth = 5f; // How far underground to go
        public float teleportDuration = 2.5f; // Total time underground
        public float behindPlayerDistance = 4f; // Distance behind player to spawn
        public List<float> regularTeleportChance = new() { 0.1f, 0.125f, 0.15f, 0.175f, 0.2f };

        // DemonKingAttackAI
        public List<float> rangedAttackChance = new() { 0.1f, 0.125f, 0.15f, 0.175f, 0.2f };

        // SwordHitbox
        public List<int> damage = new() { 5, 6, 7, 8, 9 };
        public float bounceBackStrength = 75f;

        // TargetDetector
        public List<float> attackRange = new() { 7f, 7.25f, 7.5f, 7.75f, 8f };
        public List<float> attackInterval = new() { 1f, 0.975f, 0.95f, 0.925f, 0.9f };

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
    private SwordHitbox melee;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel;
    private int intelligence = 0;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);
        attackAI = GetComponent<DemonKingAttackAI>();
        Assert.IsNotNull(attackAI);
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
        difficultyLevel = Math.Clamp(level, 1, difficultyStatsList.stats.Count);
        DifficultyStats stats = difficultyStatsList.stats[difficultyLevel - 1];

        movement.moveSpeed = stats.moveSpeed;
        movement.chaseRange = stats.chaseRange;
        movement.turnSpeed = stats.turnSpeed;
        movement.sinkSpeed = stats.sinkSpeed;
        movement.sinkDepth = stats.sinkDepth;
        movement.teleportDuration = stats.teleportDuration;
        movement.behindPlayerDistance = stats.behindPlayerDistance;
        movement.regularTeleportChance = stats.regularTeleportChance[intelligence];
        
        attackAI.rangedAttackChance = stats.rangedAttackChance[intelligence];

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
        if (intelligence < stats.regularTeleportChance.Count)
            movement.regularTeleportChance = stats.regularTeleportChance[intelligence];
        if (intelligence < stats.rangedAttackChance.Count)
            attackAI.rangedAttackChance = stats.rangedAttackChance[intelligence];
        if (intelligence < stats.damage.Count)
            melee.damage = stats.damage[intelligence];
        if (intelligence < stats.attackRange.Count)
            detector.attackRange = stats.attackRange[intelligence];
        if (intelligence < stats.attackInterval.Count)
            detector.attackInterval = stats.attackInterval[intelligence];
    }
}
