using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class OrcMovementAI : MonoBehaviour
{
    [Header("Targets & Ranges")]
    [SerializeField] private Transform player;
    public float chaseRange = 30f;
    public float stoppingDistance = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    [SerializeField] private float turnSpeed = 600f;
    public float patrolSpeedMultiplier = 0.5f;
    [SerializeField] private float animationSpeedMultiplier = 0.3f;

    private CharacterController cc;
    private CharacterController playerCC;
    private WaypointPatroller waypointPatroller;
    private Animator animator;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        Assert.IsNotNull(cc);

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

        playerCC = player.GetComponent<CharacterController>();
        Assert.IsNotNull(playerCC);
    }

    private void Start()
    {
        waypointPatroller.characterController = cc;
        waypointPatroller.moveSpeed = patrolSpeedMultiplier * moveSpeed;
    }

    private void Update()
    {
        Vector3 to = player.position - transform.position;
        Vector3 toXZ = new(to.x, 0f, to.z);

        float centerDist = toXZ.magnitude;
        if (centerDist <= chaseRange)
        {
            waypointPatroller.StopPatrol();
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(toXZ), turnSpeed * Time.deltaTime);

            float myR = cc.radius;
            float targetR = playerCC ? playerCC.radius : 0f;
            float edgeDist = centerDist - (myR + targetR);

            float moveDisplacement = edgeDist - stoppingDistance;
            moveDisplacement = Mathf.Min(Mathf.Abs(moveDisplacement), moveSpeed * Time.deltaTime) * Mathf.Sign(moveDisplacement);

            if (Mathf.Abs(moveDisplacement) > 0.01f)
            {
                cc.Move(toXZ.normalized * moveDisplacement);
                UpdateAnimationSpeed(moveDisplacement / Time.deltaTime);
            }
            else
                UpdateAnimationSpeed(0f);
        }
        else
            waypointPatroller.StartPatrol();
    }

    private void OnWaypointPatrollerMove(Vector3 displacement)
    {
        UpdateAnimationSpeed(displacement.magnitude / Time.deltaTime);
    }

    private void UpdateAnimationSpeed(float speed)
    {
        animator.SetFloat("AnimationSpeed", speed * animationSpeedMultiplier);
        animator.SetBool("IsMoving", speed > 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        cc = GetComponent<CharacterController>();
        float approxCenterStop = stoppingDistance + (cc ? cc.radius : 0.5f) + 0.5f; 
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, approxCenterStop);
    }
}