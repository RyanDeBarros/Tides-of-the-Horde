using UnityEngine;
using UnityEngine.Assertions;

public class SkeletonMovementAI : MonoBehaviour
{
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
            }
        }
        else
            waypointPatroller.StartPatrol();
    }

    // Visualize ranges in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}