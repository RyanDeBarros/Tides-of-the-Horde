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

    [Header("Visual Settings")]
    public GameObject bubbleVisualPrefab;
    private GameObject bubbleVisualInstance;

    private float timeElapsed = 0f;
    private new SphereCollider collider;

    private void Awake()
    {
        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        collider.radius = 0f;

        // Spawn the visual
        if (bubbleVisualPrefab != null)
        {
            bubbleVisualInstance = Instantiate(bubbleVisualPrefab, transform);
            bubbleVisualInstance.transform.localPosition = Vector3.zero;
            bubbleVisualInstance.transform.localScale = Vector3.zero; // start small
        }
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration)
        {
            Destroy(gameObject);
            return;
        }

        float currentRadius = (timeElapsed < growDuration)
            ? Mathf.Lerp(0f, repelRadius, timeElapsed / growDuration)
            : repelRadius;

        collider.radius = currentRadius;

        // Update visual scale (sphere) to match collider
        if (bubbleVisualInstance != null)
        {
            bubbleVisualInstance.transform.localScale = Vector3.one * currentRadius * 2f;
            // multiply by 2 since Unity sphere scale = diameter, collider.radius = radius
        }
    }

    private void OnTriggerEnter(Collider target)
    {
        if (target.TryGetComponent<BounceBack>(out var bounceBack))
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            bounceBack.Bounce(direction, bounceBackStrength);
        }
    }

    private void OnTriggerStay(Collider target)
    {
        if (target.TryGetComponent<BounceBack>(out var bounceBack))
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0f;
            direction.Normalize();

            bounceBack.Bounce(direction, bounceBackStrength);
        }
    }

    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, collider.radius);
        }
    }
}
