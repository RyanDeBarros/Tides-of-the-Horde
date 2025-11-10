using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class TargetDetector : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool ignoreVertical = true;

    [Header("Attack Gate")]
    public float attackRange = 4.57f;
    public float attackInterval = 1.0f;

    [Header("Events")]
    public UnityEvent OnCanAttack;
    public readonly List<Func<bool>> attackConditions = new();

    private Transform player;
    private float nextAttackTime = 0f;

    void Awake()
    {
        var go = GameObject.FindWithTag(playerTag);
        if (!go) Debug.LogError($"[TargetDetector] No object tagged '{playerTag}' found!", this);
        else player = go.transform;
        Assert.IsNotNull(player);
    }

    void Update()
    {
        if (PlayerIsInRange() && Time.time >= nextAttackTime && ConditionsSatisfied())
        {
            nextAttackTime = Time.time + attackInterval;
            OnCanAttack?.Invoke();
        }
    }

    private bool ConditionsSatisfied()
    {
        return attackConditions.All(f => f());
    }

    public float DistanceToPlayer()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        if (ignoreVertical)
        {
            a.y = 0f;
            b.y = 0f;
        }
        return Vector3.Distance(a, b);
    }

    public bool PlayerIsInRange()
    {
        return DistanceToPlayer() < attackRange;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        if (ignoreVertical) center.y = 0f;
        Gizmos.DrawWireSphere(center, attackRange);
    }
#endif
}
