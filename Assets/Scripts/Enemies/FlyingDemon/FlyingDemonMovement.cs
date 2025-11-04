using UnityEngine;
using UnityEngine.Assertions;

public class FlyingDemonMovement : MonoBehaviour
{
    [Header("Move Speed")]
    public float moveSpeed = 5f;
    public float chargeSpeed = 15f;
    public float patrolSpeedMultiplier = 0.5f;

    [Header("Ranges")]
    public float chaseRange = 50f;
    public float runToRange = 15f;
    public float stoppingDistance = 3f;

    [Header("Side Step")]
    public float sideStepSpeed = 10f;
    public float sideStepDuration = 0.8f;
    public float minSideStepDelay = 1.0f;
    public float maxSideStepDelay = 2.0f;

    private FlyingDemonAnimator animator;
    private WaypointPatroller waypointPatroller;
    private CharacterController characterController;
    private Transform player;

    private float lockY = 0f;
    private int sideSteppingDir = 0;
    private float sideSteppingTimeElapsed = 0f;
    private float sideSteppingDelay = 0f;

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
                sideSteppingDelay = 0f;
                sideSteppingDir = 0;
                sideSteppingTimeElapsed = 0f;

                characterController.Move(chargeSpeed * Time.deltaTime * direction);
                animator.SetRunning();
            }
            else
            {
                UpdateSideStep();

                float displacement = distanceToPlayer - stoppingDistance;
                displacement = Mathf.Min(Mathf.Abs(displacement), moveSpeed * Time.deltaTime) * Mathf.Sign(displacement);
                characterController.Move(displacement * direction);

                if (sideSteppingDir == 0)
                {
                    if (displacement > 0.1f)
                        animator.SetMovingForward();
                    else if (displacement < -0.1f)
                        animator.SetMovingBackward();
                    else
                        animator.SetIdle();
                }
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

    private void UpdateSideStep()
    {
        if (sideSteppingDir == 0)
        {
            if (sideSteppingDelay == 0f)
                sideSteppingDelay = Random.Range(minSideStepDelay, maxSideStepDelay);

            sideSteppingTimeElapsed += Time.deltaTime;
            if (sideSteppingTimeElapsed > sideSteppingDelay)
            {
                sideSteppingDir = Mathf.RoundToInt(Random.value) * 2 - 1;
                sideSteppingDelay = 0f;
                sideSteppingTimeElapsed = 0f;
            }
        }
        else
        {
            sideSteppingTimeElapsed += Time.deltaTime;
            if (sideSteppingTimeElapsed > sideStepDuration)
            {
                sideSteppingDir = 0;
                sideSteppingTimeElapsed = 0f;
            }
            else
            {
                characterController.Move(sideStepSpeed * Time.deltaTime * sideSteppingDir * transform.right);
                if (sideStepSpeed > 0)
                    animator.SetMovingRight();
                else
                    animator.SetMovingLeft();
            }
        }
    }
}
