using UnityEngine;
using UnityEngine.AI;
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
    [SerializeField] private float animationSpeedMultiplier = 0.3f;

    private CharacterController controller;
    private NavMeshAgent agent;
    private WaypointPatroller waypointPatroller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        agent = GetComponent<NavMeshAgent>();
        Assert.IsNotNull(agent);
        // Manual movement with NavMeshAgent pathfinding
        agent.updatePosition = false;
        agent.updateRotation = false;

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);
        waypointPatroller.onMove.AddListener(OnWaypointPatrollerMove);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);

        animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);
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
                agent.nextPosition = transform.position;
                agent.SetDestination(transform.position + direction * distanceToPlayer);
                controller.Move(moveSpeed * Time.deltaTime * agent.desiredVelocity.normalized);
                UpdateAnimationSpeed(moveSpeed);
            }
            else
                UpdateAnimationSpeed(0f);
        }
        else
            waypointPatroller.StartPatrol();
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        UpdateAnimationSpeed(Time.deltaTime > 1e-5f ? displacement.magnitude / Time.deltaTime : 0f);
    }

    private void UpdateAnimationSpeed(float currentSpeed)
    {
        animator.SetFloat("AnimationSpeed", currentSpeed * animationSpeedMultiplier);
        animator.SetBool("IsMoving", currentSpeed > 0.1f);
    }
}