using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DragonDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [Serializable]
    public class DifficultyStats
    {
        // DragonMovement
        public float moveSpeed = 5f;
        public float chaseRange = 40f;
        public float patrolSpeedMultiplier = 0.5f;

        // DragonAOEAttack
        public float aoeRadius = 10f;
        public float aoeFillSpeed = 20f;
        public int damage = 5;
        public float postExplosionDelay = 1f;
        public float cooldown = 3f;

        // TargetDetector
        public float attackRange = 5f;
        public float attackInterval = 1f;

        // Health
        public int maxHealth = 100;

        // BounceBack
        public float bounceBackResistance = 3f;
        public float bounceBackDuration = 0.1f;

        // RewardOnDeath
        public int reward = 5;
    }

    [Serializable]
    public class DifficultyStatsList
    {
        public List<DifficultyStats> stats;
    }

    [SerializeField] private TextAsset statsFile;

    private static DifficultyStatsList difficultyStatsList = null;

    private DragonMovement movement;
    private DragonAOEAttack attacker;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        movement = GetComponent<DragonMovement>();
        Assert.IsNotNull(movement);
        attacker = GetComponent<DragonAOEAttack>();
        Assert.IsNotNull(attacker);
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
        movement.patrolSpeedMultiplier = stats.patrolSpeedMultiplier;
        
        attacker.aoeRadius = stats.aoeRadius;
        attacker.aoeFillSpeed = stats.aoeFillSpeed;
        attacker.damage = stats.damage;
        attacker.postExplosionDelay = stats.postExplosionDelay;
        attacker.cooldown = stats.cooldown;

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
}
