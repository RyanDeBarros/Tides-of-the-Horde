using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f; // Auto-destroy after time
    
    private Vector3 direction;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime); // Auto-destroy after lifetime
    }

    public void Initialize(Vector3 fireDirection)
    {
        direction = fireDirection.normalized;
        
        // Face the direction it's moving
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void FixedUpdate()
    {
        // Move the fireball
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            // Fallback if no Rigidbody
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Don't collide with the bishop that shot it or other enemies
        //if (other.CompareTag("Enemy")) return;
        
        // Handle collision with player
        if (other.CompareTag("Player"))
        {
            // Damage player
        }
        
        // Destroy on any collision (except enemies)
        Destroy(gameObject);
    }
}