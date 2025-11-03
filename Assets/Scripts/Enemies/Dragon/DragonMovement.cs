using UnityEngine;
using UnityEngine.Assertions;

public class DragonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chaseRange = 40f;
    public float stoppingDistance = 4f;
    public float patrolSpeedMultiplier = 0.5f;

    private CharacterController controller;
    private DragonAnimator animator;
    private DragonAOEAttack attacker;
    private WaypointPatroller waypointPatroller;
    private Transform player;

    private float lockY = 0f;

    // TODO fix death animation

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        animator = GetComponentInChildren<DragonAnimator>();
        Assert.IsNotNull(animator);

        attacker = GetComponent<DragonAOEAttack>();
        Assert.IsNotNull(attacker);

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
    }

    private void Start()
    {
        waypointPatroller.characterController = controller;
        waypointPatroller.moveSpeed = patrolSpeedMultiplier * moveSpeed;

        lockY = transform.position.y;
    }

    private void Update()
    {
        if (!controller.enabled || !animator.CanFly() || !attacker.CanMove())
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        float distanceToPlayer = direction.magnitude;
        if (distanceToPlayer <= chaseRange)
        {
            waypointPatroller.StopPatrol();

            direction.Normalize();
            transform.rotation = Quaternion.LookRotation(direction);

            float displacement = distanceToPlayer - stoppingDistance;
            displacement = Mathf.Min(Mathf.Abs(displacement), moveSpeed * Time.deltaTime) * Mathf.Sign(displacement);
            Vector3 movement = Vector3.zero;
            if (Mathf.Abs(displacement) > 0.01f)
                movement = displacement * direction;

            controller.Move(movement);
            animator.SetFlying(movement);
        }
        else
        {
            waypointPatroller.StartPatrol();
            animator.SetFlying(transform.forward);
        }

        LockY();
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
