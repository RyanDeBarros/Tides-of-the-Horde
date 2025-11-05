using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private DemonKingMovementAI movement;
    [SerializeField] private DemonKingAttackAI attackAI;

    private bool movementLocked = false;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        if (movement == null)
            movement = GetComponentInParent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);

        if (attackAI == null)
            attackAI = GetComponentInParent<DemonKingAttackAI>();
        Assert.IsNotNull(attackAI);
    }

    public void TriggerAttack1()
    {
        movementLocked = true;
        animator.SetTrigger("Attack1");
    }

    public void TriggerAttack2()
    {
        movementLocked = true;
        animator.SetTrigger("Attack2");
    }

    // Called by animator
    public void UnlockMovement()
    {
        movementLocked = false;
    }

    public bool IsMovementLocked()
    {
        return movementLocked;
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void TriggerGetHit()
    {
        movementLocked = true;
        animator.SetTrigger("GetHit");
    }

    public void OnGetHitEnd()
    {
        movement.StartTeleportSequence();
        movementLocked = false;
    }

    public void TriggerTelegraph()
    {
        animator.SetTrigger("Telegraph");
        movementLocked = false;
    }

    public void OnTelegraphEnd()
    {
        movementLocked = true;
    }
}
