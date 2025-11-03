using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class DemonKingMovementAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public DemonKingAnimator animator;
    public GameObject planeVFX; // The plane object to show during teleport

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;
    public float turnSpeed = 360f;

    [Header("Teleport Settings")]
    public float sinkSpeed = 3f; // Speed at which boss sinks into ground
    public float sinkDepth = 5f; // How far underground to go
    public float teleportDuration = 2f; // Total time underground
    public float behindPlayerDistance = 3f; // Distance behind player to spawn

    private bool isTeleporting = false;
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 originalPosition;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        if (!animator)
            animator = GetComponentInChildren<DemonKingAnimator>();
        Assert.IsNotNull(animator);

        if (!player)
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
            healthComponent.onHealthThresholdReached.AddListener(StartTeleportSequence);
    }

    void Update()
    {
        if (animator.IsMovementLocked() || isTeleporting)
        {
            animator.SetSpeed(0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool shouldChase = distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance;

        if (shouldChase) ChasePlayer();
        else animator.SetSpeed(0f); // Idle

        controller.Move(velocity * Time.deltaTime);
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position);
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            direction.Normalize();

            // Rotate towards the player smoothly
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Rotate a portion of the way each frame based on turnSpeed (degrees per second)
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );

            // Move towards the player
            Vector3 movement = moveSpeed * Time.deltaTime * direction;
            controller.Move(movement);

            // Animation blending: walk only while moving
            animator.SetSpeed(movement.magnitude > 0.01f ? 1f : 0f);
        }
        else
            animator.SetSpeed(0f);
    }

    public bool IsTeleporting()
    {
        return isTeleporting;
    }

    // Called when health reaches a 10% threshold
    public void StartTeleportSequence()
    {
        if (!isTeleporting)
        {
            StartCoroutine(TeleportBehindPlayer());
        }
    }

    private IEnumerator TeleportBehindPlayer()
    {
        isTeleporting = true;

        // Disable character controller to manually control position
        controller.enabled = false;

        // Show plane VFX
        planeVFX.SetActive(true);

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
        if (lookDirection != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDirection);

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
    }
}