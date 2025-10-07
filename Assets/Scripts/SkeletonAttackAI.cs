using UnityEngine;

public class SkeletonAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private MeleeHitbox hitbox;   

    public string attackTrigger = "Fire";
    public string attackTag = "Attack";
    public string fallbackAttackStateName = "Attack01";

    int trigHash;
    bool wasInAttack = false;

    void Reset() { if (!animator) animator = GetComponentInChildren<Animator>(); }
    void Awake() { if (!animator) animator = GetComponentInChildren<Animator>(); trigHash = Animator.StringToHash(attackTrigger);
                   if (!hitbox) hitbox = GetComponentInChildren<MeleeHitbox>();
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
        bool inAttack = IsAttackingNow();

        
        if (wasInAttack && !inAttack && hitbox != null)
        {
            Debug.Log("[AI] Attack ended ¡ú DealDamage()");
            hitbox.DealDamage();
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
