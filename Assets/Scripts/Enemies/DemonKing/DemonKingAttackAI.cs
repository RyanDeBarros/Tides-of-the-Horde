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

    // TODO add to difficulty
    public float comboProbability = 0.2f;

    [Header("Ranged Attack")]
    public float minRangedAttackDelay = 4f;
    public float maxRangedAttackDelay = 20f;
    private float rangedAttackDelayElapsed = 0f;
    private float rangedAttackDelay = 0f;

    [Header("Spike Traps")]
    // TODO add to difficulty
    public int spikesInitialDamage = 5;
    public float spikesDamageOverTime = 5f;
    public float spikesSlowingFactor = 0.75f;
    public float spikesFocusRadius = 20f;
    public float spikesTelegraphDuration = 2f;
    public float spikesRisingDuration = 0.5f;
    public float spikesStayingDuration = 0.5f;
    public float spikesFallingDuration = 0.2f;
    public int spikeTrapCount = 5;

    private enum RangedAttackState
    {
        None,
        Telegraph,
        Cooldown
    }

    private RangedAttackState rangedAttackState = RangedAttackState.None;
    private float timeElapsed = 0f;

    private Transform player;
    private List<SpikeTrap> allSpikeTraps;
    private readonly List<SpikeTrap> activeSpikeTraps = new();

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

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        Assert.IsNotNull(go);
        player = go.transform;
        Assert.IsNotNull(player);

        allSpikeTraps = FindObjectsByType<SpikeTrap>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    private void Start()
    {
        rangedAttackDelay = Random.Range(minRangedAttackDelay, maxRangedAttackDelay);
    }

    private void Update()
    {
        switch (rangedAttackState)
        {
            case RangedAttackState.None:
                if (!detector.PlayerIsInRange() && !animator.IsMovementLocked() && !movement.IsTeleporting())
                {
                    rangedAttackDelayElapsed += Time.deltaTime;
                    if (rangedAttackDelayElapsed >= rangedAttackDelay)
                        StartRangedAttackTelegraph();
                }
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
        // Give priority to spike traps that are near player
        activeSpikeTraps.Clear();
        var availableSpikeTraps = allSpikeTraps
            .Where(spikeTrap => spikeTrap.IsInactive())
            .Select(spikeTrap => new {
                spikeTrap,
                distance = Vector3.Distance(
                    new Vector3(spikeTrap.transform.position.x, 0f, spikeTrap.transform.position.z),
                    new Vector3(player.position.x, 0f, player.position.z)
                )
            })
            .OrderBy(x => x.distance);
        
        int count = System.Math.Min(spikeTrapCount, availableSpikeTraps.Count());
        if (count <= 0) return;

        // Add closest to player
        --count;
        activeSpikeTraps.Add(availableSpikeTraps.First().spikeTrap);

        if (count > 0)
        {
            availableSpikeTraps.ToList().RemoveAt(0);
            var closeSpikeTraps = availableSpikeTraps.Where(x => x.distance < spikesFocusRadius).Select(x => x.spikeTrap);
            
            if (count <= closeSpikeTraps.Count())
            {
                // Randomly add from close spike traps
                activeSpikeTraps.AddRange(closeSpikeTraps.ToList().GetRandomDistinctElements(count));
            }
            else
            {
                // Add all close spike traps + remaining nearby spike traps
                activeSpikeTraps.AddRange(closeSpikeTraps.ToList());
                activeSpikeTraps.AddRange(availableSpikeTraps.Select(x => x.spikeTrap).Skip(count).Take(count - closeSpikeTraps.Count()).ToList());
            }
        }

        activeSpikeTraps.ForEach(spikes => {
            spikes.telegraphDuration = spikesTelegraphDuration;
            spikes.risingDuration = spikesRisingDuration;
            spikes.stayingDuration = spikesStayingDuration;
            spikes.fallingDuration = spikesFallingDuration;
            spikes.initialDamage = spikesInitialDamage;
            spikes.damageOverTime = spikesDamageOverTime;
            spikes.slowingFactor = spikesSlowingFactor;
            spikes.Execute();
        });

        rangedAttackState = RangedAttackState.Telegraph;
        timeElapsed = 0f;
        animator.TriggerTelegraph();
        rangedAttackDelayElapsed = 0f;
        rangedAttackDelay = Random.Range(minRangedAttackDelay, maxRangedAttackDelay);
    }

    // Called by TargetDetector
    public void TriggerAttack()
    {
        if (movement.IsTeleporting() || animator.IsMovementLocked()) return;

        if (Random.value < comboProbability)
            animator.TriggerComboAttack();
        else
        {
            if (Mathf.RoundToInt(Random.value) == 0)
                animator.TriggerAttack1();
            else
                animator.TriggerAttack2();
        }
    }

    public bool IsMovementLocked()
    {
        return rangedAttackState == RangedAttackState.Telegraph;
    }
}
