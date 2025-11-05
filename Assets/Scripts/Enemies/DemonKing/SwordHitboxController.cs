using UnityEngine;
using UnityEngine.Assertions;

public class SwordHitboxController : MonoBehaviour
{
    public int damage = 1;
    public float bounceBackStrength = 5f;
    [SerializeField] private SwordHitbox hitbox;

    private bool playerHit = false;

    private void Awake()
    {
        Assert.IsNotNull(hitbox);
        hitbox.onCollision.AddListener(OnPlayerCollision);
    }

    private void Start()
    {
        hitbox.DisableCollision();
    }

    private void OnPlayerCollision(Transform player)
    {
        if (playerHit) return;

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

    // Animation Events
    public void EnableHitbox()
    {
        playerHit = false;
        hitbox.EnableCollision();
    }

    public void DisableHitbox()
    {
        hitbox.DisableCollision();
    }

    // Draw wireframe in editor
    private void OnDrawGizmosSelected()
    {
        if (!hitbox.TryGetComponent(out BoxCollider hitCollider)) return;

        Gizmos.color = Color.red;
        // Set the matrix to match the collider's transform
        Gizmos.matrix = hitCollider.transform.localToWorldMatrix;
        // Draw the wire cube using local center and size
        Gizmos.DrawWireCube(hitCollider.center, hitCollider.size);
    }
}
