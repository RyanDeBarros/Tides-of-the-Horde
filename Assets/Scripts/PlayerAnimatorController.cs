using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    Animator playerAnimator;

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
        // Check if any movement key is currently pressed
        bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                      || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        // Running if holding shift while walking
        bool isRunning = isWalking && Input.GetKey(KeyCode.LeftShift);

        // Update animator parameters directly
        playerAnimator.SetBool("isWalkingFWD", isWalking);
        playerAnimator.SetBool("isRunning", isRunning);

        // Attack (Left Mouse Button)
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            playerAnimator.SetTrigger("Attack1");
        }
    }
}
