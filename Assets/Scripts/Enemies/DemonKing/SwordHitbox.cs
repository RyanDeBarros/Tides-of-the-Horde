using UnityEngine;

public class SwordHitbox : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Hitbox Collider")]
    public BoxCollider hitCollider;

    private void Start()
    {
        hitCollider.enabled = false;
    }

    // Animation Event: Call this during hit frame
    public void EnableHitbox()
    {
        hitCollider.enabled = true;
    }

    // Animation Event: Call this after hit frame
    public void DisableHitbox()
    {
        hitCollider.enabled = false;
    }

    // Collision trigger when player gets hit
    private void OnTriggerEnter(Collider other)
    {
        // Only proceed if the collided object is on the "Player" layer
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }

}
