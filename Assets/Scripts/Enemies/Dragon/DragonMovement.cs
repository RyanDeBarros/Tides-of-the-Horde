using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class DragonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chaseRange = 40f;
    public float stoppingDistance = 4f;
    public float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float turnSpeed = 800f;
    [SerializeField] private float navRecalculateThreshold = 0.1f;

    private CharacterController controller;
    private NavMover mover;
    private DragonAnimator animator;
    private DragonAOEAttack attacker;
    private WaypointPatroller waypointPatroller;
    private Transform player;

    private float lockY = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        mover = GetComponent<NavMover>();
        Assert.IsNotNull(mover);

        animator = GetComponentInChildren<DragonAnimator>();
        Assert.IsNotNull(animator);

        attacker = GetComponent<DragonAOEAttack>();
        Assert.IsNotNull(attacker);

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);
        waypointPatroller.onMove.AddListener(OnWaypointPatrollerMove);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
    }

    private void Start()
    {
        lockY = transform.position.y;
    }

    private void Update()
    {
        if (!controller.enabled || !animator.CanFly() || !attacker.CanMove())
            return;

        Vector3 displacement = player.position - transform.position;
        displacement.y = 0;

        if (displacement.magnitude <= chaseRange)
        {
            waypointPatroller.StopPatrol();

            Vector3 movement = mover.MoveController(displacement, stoppingDistance, moveSpeed, turnSpeed);
            animator.SetFlying(movement);
        }
        else
            waypointPatroller.StartPatrol();

        LockY();
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        Vector3 movement = mover.MoveController(displacement, moveSpeed, turnSpeed);
        animator.SetFlying(movement);
    }

    private void LockY()
    {
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
