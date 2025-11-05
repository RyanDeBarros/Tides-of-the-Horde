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

        activeSpikeTraps.ForEach(spikes => spikes.Execute(spikesTelegraphDuration, spikesRisingDuration, spikesStayingDuration, spikesFallingDuration, rangedDamage));
        rangedAttackState = RangedAttackState.Telegraph;
        timeElapsed = 0f;
        animator.TriggerTelegraph();
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
