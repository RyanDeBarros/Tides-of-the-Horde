using UnityEngine;
using UnityEngine.Assertions;

public class DemonKingAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private DemonKingMovementAI movement;
    [SerializeField] private DemonKingAttackAI attackAI;
    [SerializeField] private AudioClip attack1Clip;
    [SerializeField] private AudioClip attack2Clip;
    [SerializeField] private AudioSource audioSource;

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

        if (audioSource == null)
            audioSource = GetComponentInParent<AudioSource>();
        Assert.IsNotNull(audioSource);
    }

    public void TriggerAttack1()
    {
        movementLocked = true;
        animator.SetTrigger("Attack1");
        if (attack1Clip != null)
            audioSource.PlayOneShot(attack1Clip);
    }

    public void TriggerAttack2()
    {
        movementLocked = true;
        animator.SetTrigger("Attack2");
        if (attack2Clip != null)
            audioSource.PlayOneShot(attack2Clip);
    }

    public void TriggerComboAttack()
    {
        movementLocked = true;
        animator.SetTrigger("ComboAttack");
        if (attack1Clip != null)
            audioSource.PlayOneShot(attack1Clip);
    }

    // Called by animator
    public void OnComboAttackReadjust()
    {
        movement.FacePlayer();
        if (attack2Clip != null)
            audioSource.PlayOneShot(attack2Clip);
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
        // TODO get hit sfx for bosses
    }

    public void TriggerTelegraph()
    {
        animator.SetTrigger("Telegraph");
        movementLocked = true;
    }

    public void OnTelegraphEnd()
    {
        movementLocked = false;
    }
}
