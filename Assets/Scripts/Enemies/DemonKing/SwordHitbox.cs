using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class SwordHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    [Space]
    public float bounceBackStrength = 5f;

    [Header("Hitbox Collider (reference only)")]
    public BoxCollider hitCollider; // Disabled, used only for box dimensions

    [Header("Detection")]
    public LayerMask playerLayer;

    private bool playerHit = false;

    private bool attacking = false;

    private void Awake()
    {
        Assert.IsNotNull(hitCollider);
    }

    private void Start()
    {
        hitCollider.enabled = false;
    }

    private void Update()
    {
        if (!attacking || playerHit) return;

        // TODO use rigid body so as to use Continuous collision detection

        Vector3 center = hitCollider.transform.TransformPoint(hitCollider.center);
        Vector3 halfExtents = Vector3.Scale(hitCollider.size * 0.5f, hitCollider.transform.lossyScale);

        Collider[] hits = new Collider[1];
        if (Physics.OverlapBoxNonAlloc(center, halfExtents, hits, hitCollider.transform.rotation, playerLayer) > 0)
        {
            Transform player = hits[0].transform;

            Health health = player.GetComponent<Health>();
            Assert.IsNotNull(health);
            health.TakeDamage(damage);

            // Bounce back the player
            if (player.TryGetComponent<BounceBack>(out var bounceBack))
            {
                Vector3 direction = player.position - transform.position;
                direction.y = 0; // Keep bounce horizontal
                bounceBack.Bounce(direction.normalized, bounceBackStrength);
            }

            playerHit = true;
        }
    }

    // Animation Events
    public void EnableHitbox()
    {
        playerHit = false;
        attacking = true;
    }

    public void DisableHitbox()
    {
        attacking = false;
    }

    // Draw wireframe in editor
    private void OnDrawGizmosSelected()
    {
        if (!hitCollider) return;

        Gizmos.color = Color.red;
        // Set the matrix to match the collider's transform
        Gizmos.matrix = hitCollider.transform.localToWorldMatrix;
        // Draw the wire cube using local center and size
        Gizmos.DrawWireCube(hitCollider.center, hitCollider.size);
    }
}
