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
    private NavMover mover;
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

        mover = GetComponent<NavMover>();
        Assert.IsNotNull(mover);

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

        Vector3 displacement = player.position - transform.position;
        displacement.y = 0;

        if (displacement.magnitude <= chaseRange)
        {
            waypointPatroller.StopPatrol();

            if (displacement.magnitude >= runToRange)
            {
                ResetSideStep();
                mover.MoveController(displacement, chargeSpeed, turnSpeed);
                animator.SetRunning();
            }
            else
            {
                float movement = Vector3.Dot(mover.MoveController(displacement, stoppingDistance, moveSpeed, turnSpeed), displacement.normalized);
                UpdateSideStep();
                if (sideSteppingDir == 0)
                {
                    if (movement > 0.1f)
                        animator.SetMovingForward();
                    else if (movement < -0.1f)
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

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        mover.MoveController(displacement, moveSpeed, turnSpeed);
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
        mover.LookInDirection(player.position - transform.position, turnSpeed);
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
                Vector3 displacement = sideStepSpeed * Time.deltaTime * sideSteppingDir * transform.right;
                mover.RestrictMovement(ref displacement);
                controller.Move(displacement);
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
