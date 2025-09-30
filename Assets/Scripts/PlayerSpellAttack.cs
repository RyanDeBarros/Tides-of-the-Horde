using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerSpellAttack : MonoBehaviour
{
    private PlayerAnimatorController animator;
    private Health health; 

    private void Awake()
    {
        animator = GetComponentInChildren<PlayerAnimatorController>();
        health = GetComponent<Health>();
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
        if (Input.GetMouseButtonDown(1))  // 1 = right click
        {
            health.TakeDamage(10);
        }
    }
}
