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
    private bool playerHit = false;
    private bool swinging = false;

    private void Awake()
    {
        Assert.IsNotNull(colliderRoot);
        colliders.AddRange(colliderRoot.GetComponents<SphereCollider>());
        colliders.AddRange(colliderRoot.GetComponentsInChildren<SphereCollider>());
    }

    public void BeginSwing()
    {
        playerHit = false;
        swinging = true;
    }

    public void EndSwing()
    {
        swinging = false;
    }

    public void Update()
    {
        if (!swinging || playerHit)
            return;

        foreach (SphereCollider collider in colliders)
        {
            float maxScale = Mathf.Max(collider.transform.lossyScale.x, collider.transform.lossyScale.y, collider.transform.lossyScale.z);
            Collider[] cols = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(collider.transform.TransformPoint(collider.center), collider.radius * maxScale, cols, targetMask) > 0)
            {
                Health health = cols[0].GetComponentInParent<Health>();
                Assert.IsNotNull(health);
                health.TakeDamage(damage);
                playerHit = true;
                break;
            }
        }
    }
}
