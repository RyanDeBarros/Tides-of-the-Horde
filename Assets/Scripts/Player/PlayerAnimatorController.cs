using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Gravity gravity;

    private readonly List<ICallbackOnAttack1Climax> onAttack1ClimaxCallbacks = new();
    private readonly List<ICallbackOnAttack2Climax> onAttack2ClimaxCallbacks = new();

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        gravity = GetComponentInParent<Gravity>();
        if (gravity == null)
        {
            Debug.LogError("gravity script not found on " + gameObject.name);
        }
    }

    public void SetWalking(bool isWalking)
    {
        animator.SetBool("isWalkingFWD", isWalking && gravity.GetIsGrounded());
    }

    public void SetRunning(bool isRunning)
    {
        animator.SetBool("isRunning", isRunning && gravity.GetIsGrounded());
    }

    public void SetAttackAnimSpeed(float speedMultiplier)
    {
        animator.SetFloat("AttackAnimSpeed", speedMultiplier);
    }

    public void ExecuteAttack1()
    {
        animator.SetTrigger("Attack1");
    }

    public void RegisterOnAttack1Climax(ICallbackOnAttack1Climax callback)
    {
        onAttack1ClimaxCallbacks.Add(callback);
    }

    public void OnAttack1Climax()
    {
        foreach (var c in onAttack1ClimaxCallbacks)
            c.OnAttack1Climax();
    }

    public void ExecuteAttack2()
    {
        animator.SetTrigger("Attack2");
    }

    public void RegisterOnAttack2Climax(ICallbackOnAttack2Climax callback)
    {
        onAttack2ClimaxCallbacks.Add(callback);
    }

    public void OnAttack2Climax()
    {
        foreach (var c in onAttack2ClimaxCallbacks)
            c.OnAttack2Climax();
    }

    public IEnumerator PlayDeathAnimation()
    {
        animator.SetTrigger("Die");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            yield return null;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
