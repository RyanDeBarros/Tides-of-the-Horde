using UnityEngine;
using UnityEngine.Assertions;

public class MeleeAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Animator")]
    public string attackTrigger = "Fire";

    int trigHash;

    void Reset()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);
    }

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);
        trigHash = Animator.StringToHash(attackTrigger);
    }

    /// Call this from your AI/BT to start an attack
    public void TryAttack()
    {
        animator.ResetTrigger(trigHash);
        animator.SetTrigger(trigHash);
    }
}
