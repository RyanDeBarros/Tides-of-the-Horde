using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

class DemonKingAttackAI : MonoBehaviour
{
    [SerializeField] private GameObject spikesPrefab;
    [SerializeField] private DemonKingMovementAI movement;
    [SerializeField] private DemonKingAnimator animator;
    [SerializeField] private TargetDetector detector;

    public float rangedAttackChance = 0.1f;
    // TODO add to difficulty
    public float telegraphDuration = 1f;
    public int rangedDamage = 10;
    public float spikesRisingSpeed = 5f;

    private enum RangedAttackState
    {
        None,
        Telegraph,
        SpikesRising
    }

    private RangedAttackState rangedAttackState = RangedAttackState.None;
    private float timeElapsed = 0f;

    private void Awake()
    {
        Assert.IsNotNull(spikesPrefab);

        if (movement == null)
            movement = GetComponent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);

        if (animator == null)
            animator = GetComponentInChildren<DemonKingAnimator>();
        Assert.IsNotNull(animator);

        if (detector == null)
            detector = GetComponent<TargetDetector>();
        Assert.IsNotNull(detector);
    }

    private void Update()
    {
        switch (rangedAttackState)
        {
            case RangedAttackState.None:
                if (!detector.PlayerIsInRange() && Random.value <= rangedAttackChance * Time.deltaTime // TODO different method for probability
                        && !animator.IsMovementLocked() && !movement.IsTeleporting())
                    StartRangedAttackTelegraph();
                break;
            case RangedAttackState.Telegraph:
                timeElapsed += Time.deltaTime;
                if (timeElapsed < telegraphDuration)
                    UpdateRangedTelegraph();
                else
                {
                    timeElapsed = 0f;
                    rangedAttackState = RangedAttackState.SpikesRising;
                }
                break;
            case RangedAttackState.SpikesRising:
                // TODO animate spikes rising and deal damage to player
                break;
        }
    }

    private void StartRangedAttackTelegraph()
    {
        rangedAttackState = RangedAttackState.Telegraph;
        timeElapsed = 0f;
        animator.TriggerTelegraph();
        // TODO init spikes
    }

    private void UpdateRangedTelegraph()
    {
        // TODO shadows on ground where spikes will emerge
    }

    // Called by TargetDetector
    public void TriggerAttack()
    {
        if (movement.IsTeleporting() || animator.IsMovementLocked()) return;

        if (Mathf.RoundToInt(Random.value) == 0)
            animator.TriggerAttack1();
        else
            animator.TriggerAttack2();
    }

    public bool IsMovementLocked()
    {
        return rangedAttackState == RangedAttackState.Telegraph;
    }
}
