using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    private Animator playerAnimator;

    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void Update()
    {
    }

    public void SetGrounded(bool isGrounded)
    {
        // TODO if isGrounded is false, then cancel walking and running animations.
    }

    public void SetWalking(bool isWalking)
    {
        playerAnimator.SetBool("isWalkingFWD", isWalking);
    }

    public void SetRunning(bool isRunning)
    {
        playerAnimator.SetBool("isRunning", isRunning);
    }

    public void ExecuteAttack1()
    {
        playerAnimator.SetTrigger("Attack1");
    }
}
