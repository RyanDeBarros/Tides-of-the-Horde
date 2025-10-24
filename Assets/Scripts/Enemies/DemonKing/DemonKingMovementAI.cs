using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class DemonKingMovementAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;

    bool movementLocked = false;

    CharacterController controller;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!animator)
            animator = GetComponentInChildren<Animator>();

        if (!player)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player || movementLocked)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool shouldChase = distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance;

        if (shouldChase)
            ChasePlayer();
        else
            animator.SetFloat("Speed", 0f); // Idle

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
        if (!animator) return;

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
}
