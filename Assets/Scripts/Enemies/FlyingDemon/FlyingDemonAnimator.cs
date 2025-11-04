using UnityEngine;
using UnityEngine.Assertions;

// TODO disable EnemyHealthBar if FlyingDemon is boss.
public class FlyingDemonAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Assert.IsNotNull(animator);
    }
}
