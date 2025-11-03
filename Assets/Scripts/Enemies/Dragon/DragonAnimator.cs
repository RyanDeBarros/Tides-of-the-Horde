using UnityEngine;
using UnityEngine.Assertions;

public class DragonAnimator : MonoBehaviour
{
    private Animator animator;
    private DragonAOEAttack attacker;
    private TargetDetector detector;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);

        attacker = GetComponentInParent<DragonAOEAttack>();
        Assert.IsNotNull(attacker);

        detector = GetComponentInParent<TargetDetector>();
        Assert.IsNotNull(detector);
        detector.attackConditions.Add(CanAttack);
    }

    public void SetFlying(Vector3 direction)
    {
        int moveVec = Mathf.RoundToInt(Vector3.Dot(direction, transform.forward));
        animator.SetInteger("MoveVec", moveVec);
    }

    public void StartAttackAnimation()
    {
        Assert.IsTrue(CanAttack());
        animator.SetTrigger("Attack");
    }

    public void OnAttackCallback()
    {
        attacker.Explode();
    }

    public bool CanFly()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return state.IsName("Flying Forward") || state.IsName("Flying Backward") || state.IsName("Idle");
    }

    private bool CanAttack()
    {
        return CanFly() && !attacker.IsExploding();
    }
}
