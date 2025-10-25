using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ChallengeGiverAnimator : MonoBehaviour
{
    [SerializeField] private string spawnTrigger = "Spawn";
    [SerializeField] private string despawnTrigger = "Despawn";

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);
    }

    public IEnumerator AnimateSpawn()
    {
        animator.SetTrigger(spawnTrigger);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(spawnTrigger))
            yield return null;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    public IEnumerator AnimateDespawn()
    {
        animator.SetTrigger(despawnTrigger);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(despawnTrigger))
            yield return null;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length * 0.99f);
    }
}
