using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SniperSpell : MonoBehaviour
{
    public float lifespan = 3f;
    public int maxEnemiesCanHit = 3;
    public int damage = 20;
    public float speed = 50f;
    public float acceleration = 50f;
    public float vfxExplosionRadius = 1f;
    public GameObject explosionFX;

    private new SphereCollider collider;
    private new Rigidbody rigidbody;
    private float timeElapsed = 0f;

    private readonly HashSet<Collider> hitEnemies = new();

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(rigidbody);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > lifespan) Destroy(gameObject);

        rigidbody.MovePosition(rigidbody.position + speed * Time.deltaTime * transform.forward);
        speed += acceleration * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (!hitEnemies.Contains(target))
        {
            hitEnemies.Add(target);
            if (target.TryGetComponent<Health>(out var health))
                health.TakeDamage(damage);
            SpawnExplosion();

            if (hitEnemies.Count >= maxEnemiesCanHit) Destroy(gameObject);
        }
    }

    private void SpawnExplosion()
    {
        GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);

        if (explosion.TryGetComponent<ParticleSystem>(out var ps))
        {
            ps.Play();
            explosion.transform.localScale = Vector3.one * vfxExplosionRadius;
            Destroy(explosion, ps.main.duration);
        }
        else
        {
            Destroy(explosion);
        }
    }

    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collider.radius);
        }
    }
}
