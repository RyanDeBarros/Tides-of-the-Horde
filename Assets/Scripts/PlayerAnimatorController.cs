using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator playerAnimator;
    private Gravity gravity; 

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

    void Update()
    {
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

    public void ExecuteAttack2()
    {
        playerAnimator.SetTrigger("Attack2");
    }
}
