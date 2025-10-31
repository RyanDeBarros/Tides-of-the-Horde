using UnityEngine;
using UnityEngine.Assertions;

public class SkeletonMovementAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;

    private CharacterController controller;

    void Start()
    {
        // Get the CharacterController component
        controller = GetComponent<CharacterController>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
    }

    void Update()
    {
        if (!controller.enabled)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        // Calculate distance and decide if we should chase
        float distanceToPlayer = direction.magnitude;
        bool shouldChase = (distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance);
        direction.Normalize();
        transform.rotation = Quaternion.LookRotation(direction);

        if (shouldChase)
        {
            Vector3 movement = moveSpeed * Time.deltaTime * direction;
            controller.Move(movement);
        }
    }

    // Visualize ranges in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}