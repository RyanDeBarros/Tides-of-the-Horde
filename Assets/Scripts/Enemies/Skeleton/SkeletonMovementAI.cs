using UnityEngine;
using UnityEngine.Assertions;

public class SkeletonMovementAI : MonoBehaviour
{
     [Header("Animation Settings")]
    private Animator animator;

    public Transform player;
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;
    public float patrolSpeedMultiplier = 0.5f;

    private CharacterController controller;
    private WaypointPatroller waypointPatroller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        waypointPatroller.characterController = controller;
        waypointPatroller.moveSpeed = patrolSpeedMultiplier * moveSpeed;
    }

    private void Update()
    {
        if (!controller.enabled)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        // Calculate distance and decide if we should chase
        float distanceToPlayer = direction.magnitude;
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction);

        if (distanceToPlayer <= chaseRange)
        {
            waypointPatroller.StopPatrol();
            if (distanceToPlayer > stoppingDistance)
            {
                Vector3 movement = moveSpeed * Time.deltaTime * direction;
                controller.Move(movement);
                UpdateAnimationSpeed(moveSpeed);
            }
            else
                UpdateAnimationSpeed(0f);
        }
        else
        {
            waypointPatroller.StartPatrol();
            UpdateAnimationSpeed(moveSpeed * patrolSpeedMultiplier);
        }
    }
        private void UpdateAnimationSpeed(float currentSpeed)
    {
        if (animator == null) return;
        
        animator.SetFloat("AnimationSpeed", currentSpeed * 0.5f);

        animator.SetBool("IsMoving", currentSpeed > 0.1f);
    }
}