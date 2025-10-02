using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BubbleSpell : MonoBehaviour
{
    public float duration = 1.5f;
    public float repelRadius = 3f;
    public float growDuration = 0.2f;
    public float bounceBackStrength = 200f;

    private float timeElapsed = 0f;

    private new SphereCollider collider;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        collider.radius = 0f;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration) Destroy(gameObject);

        if (timeElapsed < growDuration)
            collider.radius = Mathf.Lerp(0f, repelRadius, timeElapsed / growDuration);
        else
            collider.radius = repelRadius;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.TryGetComponent<BounceBack>(out var bounceBack))
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            Debug.Log($"Bounce {bounceBackStrength}");
            bounceBack.Bounce(direction, bounceBackStrength);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, collider.radius);
    }
}
