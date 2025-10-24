using System.Collections.Generic;
using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Hitbox Collider (reference only)")]
    public BoxCollider hitCollider; // Disabled, used only for box dimensions

    [Header("Detection")]
    public LayerMask playerLayer;

    private readonly HashSet<Health> hitThisSwing = new();

    private bool attacking = false;

    void Start()
    {
        if (hitCollider) hitCollider.enabled = false;
    }

    void FixedUpdate()
    {
        if (!attacking || hitCollider == null) return;

        Vector3 center = hitCollider.transform.TransformPoint(hitCollider.center);
        Vector3 halfExtents = hitCollider.size * 0.5f;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, hitCollider.transform.rotation, playerLayer);

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null && !hitThisSwing.Contains(health))
            {
                health.TakeDamage(damage);
                hitThisSwing.Add(health);
            }
        }
    }

    // Animation Events
    public void EnableHitbox()
    {
        debugging("EnableHitbox");
        hitThisSwing.Clear();
        attacking = true;
    }

    public void DisableHitbox()
    {
        debugging("DisableHitbox");
        attacking = false;
        hitThisSwing.Clear();
    }

    // Optional Debugging
    private void debugging(string msg)
    {
        Debug.Log("[SwordHitbox] " + msg);
    }

    // Draw wireframe in editor
    private void OnDrawGizmosSelected()
    {
        if (!hitCollider) return;

        Gizmos.color = Color.red;
        Vector3 center = hitCollider.transform.TransformPoint(hitCollider.center);
        Vector3 halfExtents = hitCollider.size * 0.5f;
        Gizmos.matrix = Matrix4x4.TRS(hitCollider.transform.position, hitCollider.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(hitCollider.center, hitCollider.size);
    }
}
