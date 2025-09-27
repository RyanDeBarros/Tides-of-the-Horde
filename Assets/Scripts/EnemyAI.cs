using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 5f;
    public float chaseRange = 10f;
    public float stoppingDistance = 2f;

    private CharacterController controller;
    private Vector3 velocity; // Needed to track downward velocity (gravity)

    void Start()
    {
        // Get the CharacterController component
        controller = GetComponent<CharacterController>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null)
            return;

        // Calculate distance and decide if we should chase
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool shouldChase = (distanceToPlayer <= chaseRange && distanceToPlayer > stoppingDistance);

        if (shouldChase)
        {
            ChasePlayer();
        }

        // Move the controller (this applies both movement and gravity)
        controller.Move(velocity * Time.deltaTime);
    }

    void ChasePlayer()
    {
        // 1. Calculate direction towards player (ignore height difference)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 

        // 2. Create the movement vector based on direction and speed
        Vector3 movement = direction * moveSpeed * Time.deltaTime;

        // 3. Rotate to face the player
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 4. Apply the movement to the CharacterController
        controller.Move(movement);
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