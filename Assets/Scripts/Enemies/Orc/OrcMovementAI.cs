using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(CharacterController))]
public class OrcMovementAI : MonoBehaviour
{
    [Header("Targets & Ranges")]
    public Transform player;
    public float chaseRange = 30f;
    public float stoppingDistance = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float turnSpeed = 12f;
    public float patrolSpeedMultiplier = 0.5f;

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

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("No Animator found in children for Orc!");
        }

        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
        }
        if (player) playerCC = player.GetComponent<CharacterController>();
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

        Vector3 velocity = Vector3.zero;
        float currentMoveSpeed = 0f;

        float centerDist = toXZ.magnitude;
        if (centerDist <= chaseRange)
        {
            waypointPatroller.StopPatrol();
            if (Mathf.Abs(centerDist) > 0.0001f)
            {
                Quaternion face = Quaternion.LookRotation(toXZ);
                transform.rotation = Quaternion.Slerp(transform.rotation, face, turnSpeed * Time.deltaTime);
            }

            float myR = cc.radius;
            float targetR = playerCC ? playerCC.radius : 0f;
            float edgeDist = centerDist - (myR + targetR);

            float moveDisplacement = edgeDist - stoppingDistance;
            moveDisplacement = Mathf.Min(Mathf.Abs(edgeDist - stoppingDistance), moveSpeed * Time.deltaTime) * Mathf.Sign(moveDisplacement);
            
            if (Mathf.Abs(moveDisplacement) > 0.01f)
            {
                velocity += toXZ.normalized * moveDisplacement / Time.deltaTime;
                cc.Move(velocity * Time.deltaTime);
                currentMoveSpeed = moveSpeed;
            }
        }
        else
        {
            waypointPatroller.StartPatrol();
            currentMoveSpeed = moveSpeed * patrolSpeedMultiplier;
        }
        UpdateAnimationSpeed(currentMoveSpeed);
    }

    private void UpdateAnimationSpeed(float speed)
    {
        if (animator == null) return;
        animator.SetFloat("AnimationSpeed", speed * 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        if (!cc) cc = GetComponent<CharacterController>();
        float approxCenterStop = stoppingDistance + (cc ? cc.radius : 0.5f) + 0.5f; 
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, approxCenterStop);
    }
}