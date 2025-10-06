using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell_Blast : MonoBehaviour
{
    private MeleeSpell spell;
    public float growSpeed = 6f;
    public float moveSpeed = 10f;

    private new SphereCollider collider;

    private void Awake()
    {
        spell = GetComponentInParent<MeleeSpell>();
        Assert.IsNotNull(spell);

        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        collider.radius = 0.0f;
    }

    void Update()
    {
        collider.radius += growSpeed * Time.deltaTime;
        transform.position += moveSpeed * Time.deltaTime * transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        spell.Hit(other);
    }

    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, collider.radius);
        }
    }
}
