using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BishopDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {
        // BishopRangedAI
        public float moveSpeed = 3.5f;
        public float stoppingDistance = 5f;

        public float attackRange = 8f;
        public float attackCooldown = 2f;
        public float fireballSpeed = 12f;
        public int damagePerFireball = 2;

        // Health
        public int maxHealth = 100;

        // BounceBack
        public float bounceBackResistance = 1f;
        public float bounceBackDuration = 0.1f;

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

    private BishopRangedAI rangedAI;
    private Health health;
    private BounceBack bounceBack;
    private RewardOnDeath reward;

    private int difficultyLevel;

    private void Awake()
    {
        difficultyStatsList ??= JsonUtility.FromJson<DifficultyStatsList>(statsFile.text);

        rangedAI = GetComponent<BishopRangedAI>();
        Assert.IsNotNull(rangedAI);
        health = GetComponent<Health>();
        Assert.IsNotNull(health);
        bounceBack = GetComponent<BounceBack>();
        Assert.IsNotNull(bounceBack);
        reward = GetComponent<RewardOnDeath>();
        Assert.IsNotNull(reward);
    }

    private void Start()
    {
        DifficultyStats stats = difficultyStatsList.stats[System.Math.Clamp(difficultyLevel - 1, 0, difficultyStatsList.stats.Count - 1)];

        rangedAI.moveSpeed = stats.moveSpeed;
        rangedAI.stoppingDistance = stats.stoppingDistance;
        rangedAI.attackRange = stats.attackRange;
        rangedAI.attackCooldown = stats.attackCooldown;
        rangedAI.fireballSpeed = stats.fireballSpeed;
        rangedAI.damagePerFireball = stats.damagePerFireball;
        
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
