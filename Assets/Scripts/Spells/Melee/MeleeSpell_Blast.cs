using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell_Blast : MonoBehaviour
{
    public float growSpeed = 6f;
    public float moveSpeed = 10f;

    private new SphereCollider collider;

    private void Awake()
    {
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
        // TODO
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, collider.radius);
    }
}
