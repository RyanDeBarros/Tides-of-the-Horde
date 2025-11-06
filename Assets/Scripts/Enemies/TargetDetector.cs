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
    public string playerTag = "Player";
    public bool ignoreVertical = true;

    [Header("Attack Gate")]
    public float attackRange = 4.57f;
    public float attackInterval = 1.0f;

    [Header("Events")]
    public UnityEvent OnCanAttack;
    public UnityEvent<float> OnDistanceUpdated;

    public readonly List<Func<bool>> attackConditions = new();

    Transform player;
    float nextAttackTime;

    void Awake()
    {
        var go = GameObject.FindWithTag(playerTag);
        if (!go) Debug.LogError($"[TargetDetector] No object tagged '{playerTag}' found!", this);
        else player = go.transform;
        Assert.IsNotNull(player);
    }

    void Update()
    {
        float dist = ignoreVertical
            ? HorizontalDistance(transform.position, player.position)
            : Vector3.Distance(transform.position, player.position);

        OnDistanceUpdated?.Invoke(dist);

        if (dist <= attackRange && Time.time >= nextAttackTime && ConditionsSatisfied())
        {
            nextAttackTime = Time.time + attackInterval;
            OnCanAttack?.Invoke();
        }
    }

    private bool ConditionsSatisfied()
    {
        return attackConditions.All(f => f());
    }

    static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        a.y = 0; b.y = 0;
        return Vector3.Distance(a, b);
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
