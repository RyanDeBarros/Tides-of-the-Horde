using UnityEngine;
using UnityEngine.Assertions;

public class FlyingDemonMovement : MonoBehaviour
{
    private FlyingDemonAnimator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<FlyingDemonAnimator>();
        Assert.IsNotNull(animator);
    }
}
