using UnityEngine;
using UnityEngine.Assertions;

class DemonKingAttackAI : MonoBehaviour
{
    [SerializeField] private DemonKingMovementAI movement;
    [SerializeField] private DemonKingAnimator animator;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<DemonKingMovementAI>();
        Assert.IsNotNull(movement);

        if (animator == null)
            animator = GetComponentInChildren<DemonKingAnimator>();
        Assert.IsNotNull(animator);
    }

    // Called by TargetDetector
    public void TriggerAttack()
    {
        if (movement.IsTeleporting() || animator.IsMovementLocked()) return;

        if (Random.Range(0, 2) == 0)
            animator.TriggerAttack1();
        else
            animator.TriggerAttack2();
    }
}
