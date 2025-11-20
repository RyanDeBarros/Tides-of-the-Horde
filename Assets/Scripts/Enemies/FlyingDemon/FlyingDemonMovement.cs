using UnityEngine;
using UnityEngine.Assertions;

public class FlyingDemonMovement : MonoBehaviour
{
    [Header("Move Speed")]
    public float moveSpeed = 5f;
    public float chargeSpeed = 15f;
    public float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float turnSpeed = 360f;

    [Header("Ranges")]
    public float chaseRange = 50f;
    public float runToRange = 15f;
    public float stoppingDistance = 3.5f;

    [Header("Side Step")]
    public float sideStepSpeed = 10f;
    public float sideStepDuration = 0.8f;
    public float minSideStepCooldown = 1.5f;
    public float maxSideStepCooldown = 3.0f;

    private FlyingDemonAnimator animator;
    private FlyingDemonAttackAI attacker;
    private WaypointPatroller waypointPatroller;
    private CharacterController controller;
    private Transform player;

    private float lockY = 0f;
    private int sideSteppingDir = 0;
    private float sideSteppingTimeElapsed = 0f;
    private float sideSteppingCooldown = 0f;

    private void Awake()
    {
        animator = GetComponentInChildren<FlyingDemonAnimator>();
        Assert.IsNotNull(animator);

        attacker = GetComponent<FlyingDemonAttackAI>();
        Assert.IsNotNull(attacker);

        waypointPatroller = GetComponent<WaypointPatroller>();
        Assert.IsNotNull(waypointPatroller);
        waypointPatroller.onMove.AddListener(OnWaypointPatrollerMove);

        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
    }

    private void Start()
    {
        lockY = transform.position.y;
    }

    private void Update()
    {
        if (!controller.enabled || !attacker.AllowMovement())
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        float distanceToPlayer = direction.magnitude;
        if (distanceToPlayer <= chaseRange)
        {
            waypointPatroller.StopPatrol();

            FacePlayer();
            direction.Normalize();

            if (distanceToPlayer >= runToRange)
            {
                ResetSideStep();

                controller.Move(chargeSpeed * Time.deltaTime * direction);
                animator.SetRunning();
            }
            else
            {
                float displacement = distanceToPlayer - stoppingDistance;
                displacement = Mathf.Min(Mathf.Abs(displacement), moveSpeed * Time.deltaTime) * Mathf.Sign(displacement);
                controller.Move(displacement * direction);

                UpdateSideStep();

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
            waypointPatroller.StartPatrol();

        LockY();
    }

    private void LookInDirection(Vector3 direction)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        displacement = Mathf.Min(moveSpeed * Time.deltaTime, displacement.magnitude) * displacement.normalized;
        LookInDirection(displacement);
        controller.Move(displacement);
        animator.SetMovingForward();
    }

    public void LockY()
    {
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;
    }

    public void FacePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f;
        LookInDirection(direction);
    }

    private void UpdateSideStep()
    {
        if (sideSteppingDir == 0)
        {
            if (sideSteppingCooldown == 0f)
                sideSteppingCooldown = Random.Range(minSideStepCooldown, maxSideStepCooldown);

            sideSteppingTimeElapsed += Time.deltaTime;
            if (sideSteppingTimeElapsed > sideSteppingCooldown && !attacker.IsAttacking())
            {
                sideSteppingDir = Mathf.RoundToInt(Random.value) * 2 - 1;
                sideSteppingCooldown = 0f;
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
                controller.Move(sideStepSpeed * Time.deltaTime * sideSteppingDir * transform.right);
                if (sideStepSpeed > 0)
                    animator.SetMovingRight();
                else
                    animator.SetMovingLeft();
            }
        }
    }

    private void ResetSideStep()
    {
        sideSteppingCooldown = 0f;
        sideSteppingDir = 0;
        sideSteppingTimeElapsed = 0f;
    }

    public bool CanAttack()
    {
        return sideSteppingDir == 0;
    }
}
