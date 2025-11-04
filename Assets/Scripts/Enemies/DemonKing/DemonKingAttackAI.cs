using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

class DemonKingAttackAI : MonoBehaviour
{
    [SerializeField] private DemonKingMovementAI movement;
    [SerializeField] private DemonKingAnimator animator;

    // TODO add to difficulty
    public float attack1ProbabilityWeight = 1f;
    public float attack2ProbabilityWeight = 1f;

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

        Dictionary<Action, float> attacks = new() {
            { () => animator.TriggerAttack1(), attack1ProbabilityWeight },
            { () => animator.TriggerAttack2(), attack2ProbabilityWeight }
            // TODO ranged attack
        };
        Action attack = attacks.Keys.ToList().GetWeightedRandomElement(attacks.Values.ToList());
        attack.Invoke();
    }
}
