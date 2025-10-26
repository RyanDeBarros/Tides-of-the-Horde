using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DemonKingMovementAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject planeVFX; // The plane object to show during teleport

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;

    [Header("Teleport Settings")]
    public float sinkSpeed = 3f; // Speed at which boss sinks into ground
    public float sinkDepth = 5f; // How far underground to go
    public float teleportDuration = 2f; // Total time underground
    public float behindPlayerDistance = 3f; // Distance behind player to spawn

    bool movementLocked = false;
    bool isTeleporting = false;
    CharacterController controller;
    Vector3 velocity;
    Vector3 originalPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!player) player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Make sure plane VFX is hidden at start
        if (planeVFX) planeVFX.SetActive(false);

        // Hook up to health threshold event
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.onHealthThresholdReached.AddListener(StartTeleportSequence);
        }
    }

    void Update()
    {
        if (!player || movementLocked || isTeleporting)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool shouldChase = distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance;

        if (shouldChase) ChasePlayer();
        else animator.SetFloat("Speed", 0f); // Idle

        controller.Move(velocity * Time.deltaTime);
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0;
        direction.Normalize();

        Vector3 movement = direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        controller.Move(movement);

        // Animation blending: walk only while moving
        animator.SetFloat("Speed", movement.magnitude > 0.01f ? 1f : 0f);
    }

    // Called by TargetDetector (UnityEvent)
    public void TriggerRandomAttack()
    {
        if (!animator || isTeleporting) return;

        movementLocked = true;
        animator.SetFloat("Speed", 0f);

        if (Random.Range(0, 2) == 0)
            animator.SetTrigger("Attack1");
        else
            animator.SetTrigger("Attack2");

        // Unlock after attack duration (adjust to animation length)
        Invoke(nameof(UnlockMovement), 1f);
    }

    void UnlockMovement()
    {
        movementLocked = false;
    }

    // Called when health reaches a 10% threshold
    public void StartTeleportSequence()
    {
        if (!isTeleporting)
        {
            StartCoroutine(TeleportBehindPlayer());
        }
    }

    IEnumerator TeleportBehindPlayer()
    {
        isTeleporting = true;
        movementLocked = true;

        // Disable character controller to manually control position
        controller.enabled = false;

        // Show plane VFX
        if (planeVFX) planeVFX.SetActive(true);

        // Store original position
        originalPosition = transform.position;
        Vector3 targetSinkPosition = originalPosition - new Vector3(0, sinkDepth, 0);

        // Sink into ground
        float sinkTimer = 0f;
        float sinkDuration = sinkDepth / sinkSpeed;

        while (sinkTimer < sinkDuration)
        {
            sinkTimer += Time.deltaTime;
            float t = sinkTimer / sinkDuration;
            transform.position = Vector3.Lerp(originalPosition, targetSinkPosition, t);
            yield return null;
        }

        // Wait underground (optional - part of teleportDuration)
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
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        // Rise up from ground
        float riseTimer = 0f;
        while (riseTimer < sinkDuration)
        {
            riseTimer += Time.deltaTime;
            float t = riseTimer / sinkDuration;
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
        movementLocked = false;

    }
}