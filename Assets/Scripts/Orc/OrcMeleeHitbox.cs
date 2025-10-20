using System.Collections.Generic;
using UnityEngine;

public class OrcMeleeHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    [Header("Hitbox")]
    [SerializeField] private float range = 2.0f;
    [SerializeField] private Vector3 localOffset = new Vector3(0f, 0f, 0.8f);
    [SerializeField] private LayerMask targetMask;          
    [SerializeField] private Transform origin;

    [Header("Directional Gate")]
    [SerializeField, Range(0f, 180f)] private float halfAngle = 60f; 
    [SerializeField] private bool useAngleGate = true;


    private readonly HashSet<Health> hitThisSwing = new();

   
    public void BeginSwing() => hitThisSwing.Clear();

  
    public void EndSwing() => hitThisSwing.Clear();


    public void DealDamage()
    {
        Transform o = origin ? origin : transform;
        Vector3 center = o.TransformPoint(localOffset);

        
        Collider[] cols = Physics.OverlapSphere(center, range, targetMask);

        float cosThresh = Mathf.Cos(halfAngle * Mathf.Deg2Rad);
        Vector3 fwd = o.forward;

        for (int i = 0; i < cols.Length; i++)
        {
            var h = cols[i].GetComponentInParent<Health>();
            if (h == null) continue;
            if (hitThisSwing.Contains(h)) continue;

            
            Vector3 to = (h.transform.position - center);
            if (useAngleGate)
            {
                if (to.sqrMagnitude < 1e-6f) continue;
                float dot = Vector3.Dot(fwd, to.normalized);
                if (dot < cosThresh) continue;   
            }

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

    public void AnimationEvent_DealDamage()
    {
        DealDamage();
    }
}
