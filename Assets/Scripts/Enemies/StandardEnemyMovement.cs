using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class StandardEnemyMovement : MonoBehaviour
{
    [Header("Targets & Ranges")]
    [SerializeField] private Transform player;
    public float chaseRange = 30f;
    public float stoppingDistance = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float turnSpeed = 600f;
    [SerializeField] private float animationSpeedMultiplier = 0.3f;

    private CharacterController controller;
    private NavMeshAgent agent;
    private WaypointPatroller waypointPatroller;
    private Animator animator;

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

        animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            Assert.IsNotNull(go);
            player = go.transform;
            Assert.IsNotNull(player);
        }
    }

    private void Start()
    {
        waypointPatroller.characterController = controller;
        waypointPatroller.moveSpeed = patrolSpeedMultiplier * moveSpeed;
    }

    private void Update()
    {
        if (!controller.enabled) return;

        Vector3 displacement = player.position - transform.position;
        displacement.y = 0;

        if (displacement.magnitude <= chaseRange)
        {
            waypointPatroller.StopPatrol();
            if (displacement.magnitude > stoppingDistance)
            {
                agent.nextPosition = transform.position;
                agent.SetDestination(transform.position + displacement);
                Vector3 velocity = agent.velocity;
                velocity.y = 0f;
                LookInDirection(velocity.magnitude > 0.001f ? velocity : displacement);
                controller.Move(moveSpeed * Time.deltaTime * velocity.normalized);
                UpdateAnimationSpeed(moveSpeed);
            }
            else
            {
                LookInDirection(displacement);
                UpdateAnimationSpeed(0f);
            }
        }
        else
            waypointPatroller.StartPatrol();
    }
    private void LookInDirection(Vector3 direction)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        UpdateAnimationSpeed(Time.deltaTime > 1e-5f ? displacement.magnitude / Time.deltaTime : 0f);
    }

    private void UpdateAnimationSpeed(float speed)
    {
        animator.SetFloat("AnimationSpeed", speed * animationSpeedMultiplier);
        animator.SetBool("IsMoving", speed > 0.1f);
    }
}