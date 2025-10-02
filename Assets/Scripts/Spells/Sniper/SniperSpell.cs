using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SniperSpell : MonoBehaviour
{
    public float lifespan = 3f;
    public int damage = 20;
    public float speed = 50f;
    public float acceleration = 50f;

    private new SphereCollider collider;
    private new Rigidbody rigidbody;
    private float timeElapsed = 0f;

    private HashSet<Collider> hitEnemies = new();

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
            Health health = target.GetComponent<Health>();
            if (health != null) health.TakeDamage(damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collider.radius);
    }
}
