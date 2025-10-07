using UnityEngine;

public class MeleeHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Hitbox")]
    [SerializeField] private float range = 1.5f;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0.8f); 
    [SerializeField] private LayerMask targetMask; 
    [SerializeField] private Transform origin;     

    
    public void DealDamage()
    {
        Debug.Log("[Hitbox] DealDamage fired");

        Transform o = origin ? origin : transform;
        Vector3 center = o.TransformPoint(localOffset);

        Collider[] hits = Physics.OverlapSphere(center, range, targetMask);
        foreach (var c in hits)
        {
            var h = c.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
                // Debug.Log($"Hit {c.name} for {damage}");
            }
        }
    }

    
    private void OnDrawGizmosSelected()
    {
        Transform o = origin ? origin : transform;
        Vector3 center = o.TransformPoint(localOffset);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, range);
    }
}
