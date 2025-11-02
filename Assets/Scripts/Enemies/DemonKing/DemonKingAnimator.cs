using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private DemonKingMovementAI movement;

    private bool movementLocked = false;

    private void Awake()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        if (!movement)
            movement = GetComponentInParent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);
    }

    // Called by TargetDetector (UnityEvent)
    public void TriggerRandomAttack()
    {
        if (movement.IsTeleporting()) return;

        movementLocked = true;
        SetSpeed(0f);

        if (Random.Range(0, 2) == 0)
            animator.SetTrigger("Attack1");
        else
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
}
