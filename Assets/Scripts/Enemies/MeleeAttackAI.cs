using UnityEngine;
using UnityEngine.Assertions;

public class MeleeAttackAI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSFX;

    [Header("Animator")]
    public string attackTrigger = "Fire";

    int trigHash;

    void Awake()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);

        trigHash = Animator.StringToHash(attackTrigger);
    }

    public void TryAttack()
    {
        animator.ResetTrigger(trigHash);
        animator.SetTrigger(trigHash);

        if (attackSFX != null)
            audioSource.PlayOneShot(attackSFX);
    }
}
