using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FlyingDemonDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [Serializable]
    public class DifficultyStats
    {
        // FlyingDemonMovement
        public float moveSpeed = 5f;
        public float chargeSpeed = 15f;
        public float patrolSpeedMultiplier = 0.5f;

        public float chaseRange = 50f;
        public float runToRange = 15f;
        public float stoppingDistance = 3.5f;

        public float sideStepSpeed = 10f;
        public float sideStepDuration = 0.8f;
        public float minSideStepCooldown = 1.5f;
        public float maxSideStepCooldown = 3.0f;

        // FlyingDemonAttackAI
        public int punchDamage = 5;
        public float punchBounceBackStrength = 120f;
        public float punchProbabilityWeight = 1.0f;
        public float punchAttackRange = 5f;

        public int biteDamage = 10;
        public float biteBounceBackStrength = 100f;
        public float biteProbabilityWeight = 0.75f;
        public float biteAttackRange = 5f;

        public int chargeDamage = 8;
        public float chargeBounceBackStrength = 100f;
        public float chargeProbabilityWeight = 0.3f;
        public float chargeAttackRange = 8f;
        public float chargeBackDistance = 3.5f;
        public float chargeBackSpeed = 8f;
        public float chargeForwardSpeed = 20f;
        public float dizzyDuration = 2f;

        public float minAttackCooldown = 1.0f;
        public float maxAttackCooldown = 2.0f;

        // Health
        public int maxHealth = 1000;

        // BounceBack
        public float bounceBackResistance = 6f;
        public float bounceBackDuration = 0.1f;

        // RewardOnDeath
        public int reward = 50;
    }

    [Serializable]
    public class DifficultyStatsList
    {
        public List<DifficultyStats> stats;
    }

    [SerializeField] private TextAsset statsFile;

    private static DifficultyStatsList difficultyStatsList = null;

    private FlyingDemonMovement movement;
    private FlyingDemonAttackAI attacker;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<FlyingDemonMovement>();
        Assert.IsNotNull(movement);
        attacker = GetComponent<FlyingDemonAttackAI>();
        Assert.IsNotNull(attacker);
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
        movement.chargeSpeed = stats.chargeSpeed;
        movement.patrolSpeedMultiplier = stats.patrolSpeedMultiplier;

        movement.chaseRange = stats.chaseRange;
        movement.runToRange = stats.runToRange;
        movement.stoppingDistance = stats.stoppingDistance;

        movement.sideStepSpeed = stats.sideStepSpeed;
        movement.sideStepDuration = stats.sideStepDuration;
        movement.minSideStepCooldown = stats.minSideStepCooldown;
        movement.maxSideStepCooldown = stats.maxSideStepCooldown;

        attacker.punchDamage = stats.punchDamage;
        attacker.punchBounceBackStrength = stats.punchBounceBackStrength;
        attacker.punchProbabilityWeight = stats.punchProbabilityWeight;
        attacker.punchAttackRange = stats.punchAttackRange;

        attacker.biteDamage = stats.punchDamage;
        attacker.biteBounceBackStrength = stats.punchBounceBackStrength;
        attacker.biteProbabilityWeight = stats.punchProbabilityWeight;
        attacker.biteAttackRange = stats.punchAttackRange;

        attacker.chargeDamage = stats.chargeDamage;
        attacker.chargeBounceBackStrength = stats.chargeBounceBackStrength;
        attacker.chargeProbabilityWeight = stats.chargeProbabilityWeight;
        attacker.chargeAttackRange = stats.chargeAttackRange;
        attacker.chargeBackDistance = stats.chargeBackDistance;
        attacker.chargeBackSpeed = stats.chargeBackSpeed;
        attacker.chargeForwardSpeed = stats.chargeForwardSpeed;
        attacker.dizzyDuration = stats.dizzyDuration;

        attacker.minCooldown = stats.minAttackCooldown;
        attacker.maxCooldown = stats.maxAttackCooldown;

        health.maxHealth = stats.maxHealth;

        bounceBack.resistance = stats.bounceBackResistance;
        bounceBack.duration = stats.bounceBackDuration;

        reward.reward = stats.reward;
    }

    public int GetDifficultyLevel()
    {
        return difficultyLevel;
    }
}
