using UnityEngine;
using UnityEngine.Assertions;

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
        Destroy(gameObject, lifetime);

        rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(rb);
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
        rb.velocity = direction * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) return;
        
        if (other.CompareTag("Player") && other.TryGetComponent<Health>(out var playerHealth))
                playerHealth.TakeDamage((int)damage);

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