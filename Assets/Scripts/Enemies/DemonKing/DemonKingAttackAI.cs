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
    public int rangedDamage = 10;
    public float spikesTelegraphDuration = 2f;
    public float spikesRisingDuration = 0.5f;
    public float spikesStayingDuration = 0.5f;
    public float spikesFallingDuration = 0.2f;
    public int randomSpikeTrapCount = 4;

    private enum RangedAttackState
    {
        None,
        Telegraph,
        Cooldown
    }

    private RangedAttackState rangedAttackState = RangedAttackState.None;
    private float timeElapsed = 0f;

    private List<SpikeTrap> allSpikeTraps;
    private List<SpikeTrap> activeSpikeTraps;

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

        allSpikeTraps = FindObjectsByType<SpikeTrap>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        switch (rangedAttackState)
        {
            case RangedAttackState.None:
                if (!detector.PlayerIsInRange() && Random.value <= rangedAttackChance * Time.deltaTime // TODO different method for probability. also use a cooldown.
                        && !animator.IsMovementLocked() && !movement.IsTeleporting())
                    StartRangedAttackTelegraph();
                break;
            case RangedAttackState.Telegraph:
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= spikesTelegraphDuration)
                {
                    timeElapsed = 0f;
                    rangedAttackState = RangedAttackState.Cooldown;
                }
                break;
            case RangedAttackState.Cooldown:
                break;
        }
    }

    private void StartRangedAttackTelegraph()
    {
        rangedAttackState = RangedAttackState.Telegraph;
        timeElapsed = 0f;
        animator.TriggerTelegraph();

        // TODO give priority to spike traps that are near player within some radius
        activeSpikeTraps = allSpikeTraps.Where(spikeTrap => spikeTrap.IsInactive()).GetRandomDistinctElements(randomSpikeTrapCount);
        // TODO add extra spike trap at player position, which shouldn't overlap with other active spike traps
        activeSpikeTraps.ForEach(spikes => spikes.Execute(spikesTelegraphDuration, spikesRisingDuration, spikesStayingDuration, spikesFallingDuration, rangedDamage));
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
