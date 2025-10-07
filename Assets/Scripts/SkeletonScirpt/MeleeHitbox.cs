using System.Collections.Generic;
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

   
    private readonly HashSet<Health> hitThisSwing = new();

   
    public void BeginSwing() => hitThisSwing.Clear();

  
    public void EndSwing() => hitThisSwing.Clear();

   
    public void DealDamage()
    {
        Transform o = origin ? origin : transform;
        Vector3 center = o.TransformPoint(localOffset);

        Collider[] cols = Physics.OverlapSphere(center, range, targetMask);

        for (int i = 0; i < cols.Length; i++)
        {
            
            var h = cols[i].GetComponentInParent<Health>();
            if (h == null) continue;
            if (hitThisSwing.Contains(h)) continue; 

            h.TakeDamage(damage);
            hitThisSwing.Add(h);
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
