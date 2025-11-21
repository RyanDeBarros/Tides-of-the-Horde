using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class DemonKingMovementAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private DemonKingAnimator animator;
    [SerializeField] private DemonKingAttackAI attackAI;
    [SerializeField] private GameObject planeVFX; // The plane object to show during teleport

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float chaseRange = 60f;
    [SerializeField] private float stoppingDistance = 2.8f;
    public float turnSpeed = 800f;

    [Header("Teleport Settings")]
    public float sinkSpeed = 6f; // Speed at which boss sinks into ground
    public float riseSpeed = 8f; // Speed at which boss rises from ground
    public float sinkDepth = 5f; // How far underground to go
    public float teleportDuration = 2f; // Total time underground
    public float behindPlayerDistance = 3f; // Distance behind player to spawn
    public float minRegularTeleportDelay = 4f;
    public float maxRegularTeleportDelay = 20f;

    private bool isTeleporting = false;
    private float teleportDelayElapsed = 0f;
    private float regularTeleportDelay = 0f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip AttackSFX;
    [SerializeField] private AudioClip AttackSFX2;
    [SerializeField] private AudioClip TeleportSfX;

    private CharacterController controller;
    private NavMover mover;
    private Health health;
    private float lockY = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        mover = GetComponent<NavMover>();
        Assert.IsNotNull(mover);

        health = GetComponent<Health>();
        Assert.IsNotNull(health);

        if (attackAI == null)
            attackAI = GetComponent<DemonKingAttackAI>();
        Assert.IsNotNull(attackAI);

        if (animator == null)
            animator = GetComponentInChildren<DemonKingAnimator>();
        Assert.IsNotNull(animator);

        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                player = go.transform;
        }
        Assert.IsNotNull(player);

        // Make sure plane VFX is hidden at start
        Assert.IsNotNull(planeVFX);
        planeVFX.SetActive(false);

        // Hook up to health threshold event
        if (TryGetComponent(out Health healthComponent))
            healthComponent.onHealthThresholdReached.AddListener(GetHit);
    }

    private void Start()
    {
        regularTeleportDelay = Random.Range(minRegularTeleportDelay, maxRegularTeleportDelay);
        lockY = transform.position.y;
    }

    private void Update()
    {
        if (animator.IsMovementLocked() || isTeleporting || attackAI.IsMovementLocked())
            return;

        Vector3 myPos = transform.position;
        myPos.y = 0f;
        Vector3 playerPos = player.position;
        playerPos.y = 0f;
        float distanceToPlayer = Vector3.Distance(myPos, playerPos);

        // TODO remove chaseRange after rebasing with main (fill-json)
        if (distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance)
        {
            teleportDelayElapsed += Time.deltaTime;
            if (teleportDelayElapsed >= regularTeleportDelay)
                StartTeleportSequence();
            else
            {
                // Chase player
                mover.MoveController(player.position - transform.position, moveSpeed, turnSpeed);
                animator.SetSpeed(1f);
            }
        }
        else
            animator.SetSpeed(0f);

        LockY();
    }

    private void LockY()
    {
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;
    }

    public void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        transform.rotation = Quaternion.LookRotation(direction.normalized);
    }

    public bool IsTeleporting()
    {
        return isTeleporting;
    }

    private void GetHit()
    {
        if (health.IsAlive())
        {
            animator.TriggerGetHit();
            var difficulty = GetComponent<DemonKingDifficultyImplementer>();
            Assert.IsNotNull(difficulty);
            difficulty.GetSmarter();
        }
    }

    public void StartTeleportSequence()
    {
        if (!isTeleporting && health.IsAlive())
            StartCoroutine(TeleportBehindPlayer());
    }

    private IEnumerator TeleportBehindPlayer()
    {
        isTeleporting = true;

        // play sfx
        if (audioSource != null && TeleportSfX != null)
            audioSource.PlayOneShot(TeleportSfX);

        // Disable character controller to manually control position
        controller.enabled = false;

        // Show plane VFX
        planeVFX.SetActive(true);

        // Store original position
        Vector3 originalPosition = transform.position;
        Vector3 targetSinkPosition = originalPosition - new Vector3(0, sinkDepth, 0);

        // Sink into ground
        float sinkTimer = 0f;
        float sinkDuration = sinkDepth / sinkSpeed;

        while (sinkTimer < sinkDuration)
        {
            sinkTimer += Time.deltaTime;
            float t = Mathf.Clamp01(sinkTimer / sinkDuration);
            transform.position = Vector3.Lerp(originalPosition, targetSinkPosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(teleportDuration - sinkDuration);

        // Calculate position behind player
        Vector3 playerForward = player.forward;
        Vector3 behindPlayerPos = player.position - (playerForward * behindPlayerDistance);
        behindPlayerPos.y = player.position.y; // Match player's Y position

        // Teleport to behind player (underground)
        Vector3 spawnUnderground = behindPlayerPos - new Vector3(0, sinkDepth, 0);
        transform.position = spawnUnderground;

        // Face the player
        Vector3 lookDirection = (player.position - transform.position);
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDirection);

        // Rise up from ground
        sinkDuration = sinkDepth / riseSpeed;
        float riseTimer = 0f;
        while (riseTimer < sinkDuration)
        {
            riseTimer += Time.deltaTime;
            float t = Mathf.Clamp01(riseTimer / sinkDuration);
            transform.position = Vector3.Lerp(spawnUnderground, behindPlayerPos, t);
            yield return null;
        }

        // Ensure final position is correct
        transform.position = behindPlayerPos;

        // Hide plane VFX
        if (planeVFX) planeVFX.SetActive(false);

        // Re-enable character controller
        controller.enabled = true;

        // Unlock movement
        isTeleporting = false;
        teleportDelayElapsed = 0f;
        regularTeleportDelay = Random.Range(minRegularTeleportDelay, maxRegularTeleportDelay);
    }
}