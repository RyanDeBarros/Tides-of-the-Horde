using UnityEngine;

public class SkeletonAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MeleeHitbox hitbox;

    [Header("Animator")]
    public string attackTrigger = "Fire";
    public string attackTag = "Attack";               
    public string fallbackAttackStateName = "Attack01";

    [Header("Hit Timing (normalized 0~1)")]
    [Tooltip("animation's peek hit window£¨damage only count once during each window£©")]
    public Vector2 hitWindow = new Vector2(0.35f, 0.55f);

    int trigHash;
    bool wasInAttack = false;
    bool dealtThisSwing = false;

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

    public void TryAttack()
    {
        if (!IsAttackingNow())
        {
            animator.ResetTrigger(trigHash);
            animator.SetTrigger(trigHash);
        }
    }

    void Update()
    {
        var st = animator.GetCurrentAnimatorStateInfo(0);
        bool inAttack = IsAttackingNow();

        
        if (inAttack && !wasInAttack)
        {
            dealtThisSwing = false;
            if (hitbox) hitbox.BeginSwing();
        }

        
        if (inAttack && !dealtThisSwing)
        {
            float t = st.normalizedTime % 1f; // 0~1
            if (t >= hitWindow.x && t <= hitWindow.y)
            {
                hitbox?.DealDamage();
                dealtThisSwing = true;
            }
        }

        
        if (!inAttack && wasInAttack)
        {
            dealtThisSwing = false;
            if (hitbox) hitbox.EndSwing();
        }

        wasInAttack = inAttack;
    }

    bool IsAttackingNow()
    {
        var st = animator.GetCurrentAnimatorStateInfo(0);
        if (!string.IsNullOrEmpty(attackTag) && st.IsTag(attackTag)) return true;
        if (!string.IsNullOrEmpty(fallbackAttackStateName) && st.IsName(fallbackAttackStateName)) return true;
        return false;
    }
}
