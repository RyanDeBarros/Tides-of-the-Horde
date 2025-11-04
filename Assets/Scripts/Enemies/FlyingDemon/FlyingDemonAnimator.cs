using UnityEngine;
using UnityEngine.Assertions;

// TODO disable EnemyHealthBar if FlyingDemon is boss.
public class FlyingDemonAnimator : MonoBehaviour
{
    private Animator animator;
    private FlyingDemonAttackAI attacker;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);

        attacker = GetComponentInParent<FlyingDemonAttackAI>();
        Assert.IsNotNull(attacker);

        Health health = GetComponentInParent<Health>();
        Assert.IsNotNull(health);
        health.onHealthThresholdReached.AddListener(GetHitAnimation);
    }

    private void GetHitAnimation()
    {
        ResetTriggers();
        animator.SetTrigger("GetHit");
    }

    public void SetIdle()
    {
        CancelDizzy();
        CancelSideStep();
        animator.SetInteger("MoveVec", 0);
    }

    public void SetMovingForward()
    {
        CancelDizzy();
        CancelSideStep();
        animator.SetInteger("MoveVec", 1);
    }

    public void SetMovingBackward()
    {
        CancelDizzy();
        CancelSideStep();
        animator.SetInteger("MoveVec", -1);
    }

    public void SetRunning()
    {
        CancelDizzy();
        CancelSideStep();
        animator.SetInteger("MoveVec", 2);
    }

    public void SetMovingLeft()
    {
        CancelDizzy();
        CancelMove();
        animator.SetInteger("SideStep", -1);
    }

    public void SetMovingRight()
    {
        CancelDizzy();
        CancelMove();
        animator.SetInteger("SideStep", 1);
    }

    public void SetDizzy()
    {
        CancelMove();
        CancelSideStep();
        animator.SetBool("Dizzy", true);
    }

    private void CancelMove()
    {
        animator.SetInteger("MoveVec", 0);
    }

    private void CancelSideStep()
    {
        animator.SetInteger("SideStep", 0);
    }

    private void CancelDizzy()
    {
        animator.SetBool("Dizzy", false);
    }

    public void PunchAttack()
    {
        ResetTriggers();
        animator.SetTrigger("Attack1");
    }

    public void BigBiteAttack()
    {
        ResetTriggers();
        animator.SetTrigger("Attack2");
    }

    public void SmallBiteAttack()
    {
        ResetTriggers();
        animator.SetTrigger("Attack3");
    }

    private void ResetTriggers()
    {
        animator.ResetTrigger("GetHit");
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");
        animator.ResetTrigger("Attack3");
    }

    public void OnAttackEnd()
    {
        attacker.OnAttackEnd();
    }
}
