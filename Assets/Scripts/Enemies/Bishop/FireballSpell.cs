using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    
    private Vector3 direction;
    private Rigidbody rb;

    [Header("Explosion Settings")]
    public GameObject explosionFX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    public void Initialize(Vector3 fireDirection)
    {
        direction = fireDirection.normalized;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;
        
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }
        SpawnExplosion();
        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        if (!explosionFX) return;

        GameObject explosion = Instantiate(explosionFX, transform.position, Quaternion.identity);

        if (explosion.TryGetComponent<ParticleSystem>(out var ps))
        {
            ps.Play();
            Destroy(explosion, ps.main.duration);
        }
        else
        {
            Destroy(explosion, 3f);
        }
    }
}