using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSpellAttack : MonoBehaviour
{
    private PlayerAnimatorController animator;

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimatorController>();
    }

    void Start()
    {
        Assert.IsNotNull(animator);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            animator.ExecuteAttack1();
        }
    }
}
