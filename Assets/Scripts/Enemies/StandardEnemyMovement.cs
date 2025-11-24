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
    private NavMover mover;
    private WaypointPatroller waypointPatroller;
    private Animator animator;

    private float lockY = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        mover = GetComponent<NavMover>();
        Assert.IsNotNull(mover);

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
        lockY = transform.position.y;
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
                Vector3 movement = mover.MoveController(displacement, stoppingDistance, moveSpeed, turnSpeed);
                UpdateAnimationSpeedByMovement(movement);
            }
            else
            {
                mover.LookInDirection(displacement, turnSpeed);
                UpdateAnimationSpeed(0f);
            }
        }
        else
            waypointPatroller.StartPatrol();

        transform.position = new(transform.position.x, lockY, transform.position.z);
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        Vector3 movement = mover.MoveController(displacement, moveSpeed, turnSpeed);
        UpdateAnimationSpeedByMovement(movement);
    }

    private void UpdateAnimationSpeedByMovement(Vector3 movement)
    {
        UpdateAnimationSpeed(Time.deltaTime > 1e-5f ? movement.magnitude / Time.deltaTime : 0f);
    }

    private void UpdateAnimationSpeed(float speed)
    {
        animator.SetFloat("AnimationSpeed", speed * animationSpeedMultiplier);
        animator.SetBool("IsMoving", speed > 0.1f);
    }
}