using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BombSpell : MonoBehaviour
{
    public int innerDamage = 120;
    public int outerDamage = 50;
    public float bounceBackStrength = 50f;
    public float aoeRadius = 11f;
    public LayerMask enemyLayerMask;
    public GameObject explosionFX;

    public float gravity = -10f;
    public Vector3 velocity = Vector3.zero;

    private new SphereCollider collider;
    private new Rigidbody rigidbody;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        rigidbody = GetComponent<Rigidbody>();
        Assert.IsNotNull(rigidbody);
    }

    private void Update()
    {
        velocity.y += gravity * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + velocity * Time.deltaTime);
        // TODO DeathZone script that will despawn automatically if it gets too far from stage.
    }

    private void OnTriggerEnter(Collider other)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius, enemyLayerMask);
        foreach (Collider target in hits)
        {
            Vector3 direction = target.transform.position - transform.position;
            TryBouncingBack(target, new Vector3(direction.x, 0f, direction.z).normalized);
            TryDamaging(target);
        }
        SpawnExplosion();
        Destroy(gameObject);
    }

    private void TryBouncingBack(Collider target, Vector3 direction)
    {
        if (target.TryGetComponent<BounceBack>(out var bounceBack))
        {
            bounceBack.Bounce(direction, bounceBackStrength);
        }
    }

    private void TryDamaging(Collider target)
    {
        if (target.TryGetComponent<Health>(out var health))
        {
            Vector3 closestPoint = target.ClosestPoint(transform.position);
            float distance = Vector3.Distance(transform.position, closestPoint);
            int damage = Mathf.RoundToInt(Mathf.Lerp(innerDamage, outerDamage, distance / aoeRadius));
            health.TakeDamage(damage);
        }
    }

    private void SpawnExplosion()
    {
        GameObject explosion = Instantiate(explosionFX, transform.position, transform.rotation);

        if (explosion.TryGetComponent<ParticleSystem>(out var ps))
        {
            ps.Play();
            explosion.transform.localScale = Vector3.one * aoeRadius;
            Destroy(explosion, ps.main.duration);
        }
        else
        {
            Destroy(explosion);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (collider == null)
            collider = GetComponent<SphereCollider>();

        Gizmos.DrawWireSphere(transform.position, collider.radius);
    }
}
