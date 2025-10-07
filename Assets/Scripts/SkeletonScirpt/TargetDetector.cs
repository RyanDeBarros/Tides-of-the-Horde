using UnityEngine;
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

    Transform player;
    float nextAttackTime;

    void Awake()
    {
        var go = GameObject.FindWithTag(playerTag);
        if (!go) Debug.LogError($"[TargetDetector] No object tagged '{playerTag}' found!", this);
        else player = go.transform;
    }

    void Update()
    {
        if (!player) return;

        float dist = ignoreVertical
            ? HorizontalDistance(transform.position, player.position)
            : Vector3.Distance(transform.position, player.position);

        OnDistanceUpdated?.Invoke(dist);

        if (dist <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackInterval;
            OnCanAttack?.Invoke();
        }
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
