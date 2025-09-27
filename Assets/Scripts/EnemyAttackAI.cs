using UnityEngine;

public class EnemyAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;   

    public string attackTrigger = "Fire";
    public string attackTag = "Attack";
    public string fallbackAttackStateName = "Attack01";

    int trigHash;

    void Reset() { if (!animator) animator = GetComponentInChildren<Animator>(); }
    void Awake() { if (!animator) animator = GetComponentInChildren<Animator>(); trigHash = Animator.StringToHash(attackTrigger); }

    public void TryAttack()
    {
        if (!animator) return;
        if (!IsAttackingNow())
        {
            Debug.Log("[AI] TryAttack()");
            animator.ResetTrigger(trigHash);
            animator.SetTrigger(trigHash);
        }
    }

    bool IsAttackingNow()
    {
        var st = animator.GetCurrentAnimatorStateInfo(0);
        if (!string.IsNullOrEmpty(attackTag) && st.IsTag(attackTag)) return true;
        if (!string.IsNullOrEmpty(fallbackAttackStateName) && st.IsName(fallbackAttackStateName)) return true;
        return false;
    }
}
