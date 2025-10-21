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
        public float chaseRange = 10f;
        public float stoppingDistance = 2f;

        // TargetDetector
        public float attackInterval = 1f;

        // Health
        public int maxHealth = 100;

        // BounceBack
        public float bounceBackResistance = 1f;
        public float bounceBackDuration = 0.1f;

        // MeleeHitbox
    }

    [SerializeField] private List<DifficultyStats> stats;

    private SkeletonMovementAI movement;
    private TargetDetector detector;
    private Health health;
    private BounceBack bounceBack;
    private MeleeHitbox melee;

    private int difficultyLevel = 0;

    private void Awake()
    {
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
    }

    private void Start()
    {
        // TODO apply difficulty level
    }

    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;
    }
}
