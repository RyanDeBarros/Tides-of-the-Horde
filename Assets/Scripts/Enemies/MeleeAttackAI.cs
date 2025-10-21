using UnityEngine;

public class MeleeAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MeleeHitbox hitbox;

    [Header("Animator")]
    public string attackTrigger = "Fire";

    int trigHash;

    void Reset()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!hitbox) hitbox = GetComponentInChildren<MeleeHitbox>();
    }

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!hitbox) hitbox = GetComponentInChildren<MeleeHitbox>();
        trigHash = Animator.StringToHash(attackTrigger);
    }

    /// Call this from your AI/BT to start an attack
    public void TryAttack()
    {
        hitbox?.BeginSwing();           
        animator.ResetTrigger(trigHash);
        animator.SetTrigger(trigHash);
    }
}
