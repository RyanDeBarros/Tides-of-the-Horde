using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeHitbox : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] public int damage = 1;

    [Header("Hitbox")]
    [SerializeField] private GameObject colliderRoot;
    [SerializeField] private LayerMask targetMask;
    
    private readonly List<SphereCollider> colliders = new();
    private readonly HashSet<Health> hitThisSwing = new();
    private bool swinging = false;

    private void Awake()
    {
        Assert.IsNotNull(colliderRoot);
        colliders.AddRange(colliderRoot.GetComponentsInChildren<SphereCollider>());
    }

    public void BeginSwing()
    {
        hitThisSwing.Clear();
        swinging = true;
    }

    public void EndSwing()
    {
        swinging = false;
        hitThisSwing.Clear();
    }

    public void FixedUpdate()
    {
        if (!swinging)
            return;

        colliders.ForEach(collider => {
            Collider[] cols = Physics.OverlapSphere(collider.transform.TransformPoint(collider.center), collider.radius, targetMask);
            foreach (Collider col in cols)
            {
                Health h = col.GetComponentInParent<Health>();
                if (h != null && !hitThisSwing.Contains(h))
                {
                    h.TakeDamage(damage);
                    hitThisSwing.Add(h);
                }
            }
        });
    }
}
