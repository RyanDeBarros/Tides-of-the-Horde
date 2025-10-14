using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell_Shockwave : MonoBehaviour
{
    private MeleeSpell spell;
    public float growSpeed = 10f;

    private new SphereCollider collider;

    private void Awake()
    {
        spell = GetComponentInParent<MeleeSpell>();
        Assert.IsNotNull(spell);

        collider = GetComponent<SphereCollider>();
        Assert.IsNotNull(collider);
        collider.radius = 0.0f;
    }

    void Update()
    {
        collider.radius += growSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        spell.Hit(other);
    }

    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, collider.radius);
        }
    }
}
