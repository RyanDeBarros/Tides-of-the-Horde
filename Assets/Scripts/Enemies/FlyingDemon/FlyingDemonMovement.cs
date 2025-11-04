using UnityEngine;
using UnityEngine.Assertions;

public class FlyingDemonMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float chargeSpeed = 15f;
    public float patrolSpeedMultiplier = 0.5f;

    public float chaseRange = 50f;
    public float runToRange = 15f;
    public float stoppingDistance = 3f;

    private FlyingDemonAnimator animator;
    private WaypointPatroller waypointPatroller;
    private CharacterController characterController;
    private Transform player;

    private float lockY = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<FlyingDemonAnimator>();
        Assert.IsNotNull(animator);

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);

        characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(characterController);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
    }

    private void Start()
    {
        waypointPatroller.moveSpeed = moveSpeed * patrolSpeedMultiplier;
        lockY = transform.position.y;
    }

    private void Update()
    {
        if (!characterController.enabled)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        float distanceToPlayer = direction.magnitude;
        if (distanceToPlayer <= chaseRange)
        {
            waypointPatroller.StopPatrol();
            transform.rotation = Quaternion.LookRotation(direction);
            direction.Normalize();

            if (distanceToPlayer >= runToRange)
            {
                characterController.Move(chargeSpeed * Time.deltaTime * direction);
                animator.SetRunning();
            }
            else
            {
                float displacement = distanceToPlayer - stoppingDistance;
                displacement = Mathf.Min(Mathf.Abs(displacement), moveSpeed * Time.deltaTime) * Mathf.Sign(displacement);
                characterController.Move(displacement * direction);

                if (displacement > 0.1f)
                    animator.SetMovingForward();
                else if (displacement < -0.1f)
                    animator.SetMovingBackward();
                else
                    animator.SetIdle();
            }
        }
        else
        {
            waypointPatroller.StartPatrol();
            animator.SetMovingForward();
        }

        LockY();
    }

    private void LockY()
    {
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;
    }
}
