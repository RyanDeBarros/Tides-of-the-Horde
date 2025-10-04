using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator playerAnimator;
    private Gravity gravity;

    private List<ICallbackOnAttack1Climax> onAttack1ClimaxCallbacks = new();
    private List<ICallbackOnAttack2Climax> onAttack2ClimaxCallbacks = new();

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null)
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
        playerAnimator.SetBool("isWalkingFWD", isWalking && gravity.GetIsGrounded());
    }

    public void SetRunning(bool isRunning)
    {
        playerAnimator.SetBool("isRunning", isRunning && gravity.GetIsGrounded());
    }

    public void SetAttackAnimSpeed(float speedMultiplier)
    {
        playerAnimator.SetFloat("AttackAnimSpeed", speedMultiplier);
    }

    public void ExecuteAttack1()
    {
        playerAnimator.SetTrigger("Attack1");
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
        playerAnimator.SetTrigger("Attack2");
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
}
