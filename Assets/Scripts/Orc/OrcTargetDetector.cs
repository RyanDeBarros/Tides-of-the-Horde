using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class OrcTargetDetector : MonoBehaviour
{
    [Header("Target")]
    public string playerTag = "Player";
    public bool ignoreVertical = true;

    [Header("Attack Gate (edge distance)")]
    public float attackRange = 0.6f;   
    public float attackInterval = 1.0f;
    public float hysteresis = 0.1f;    

    [Header("Events")]
    public UnityEvent OnCanAttack;
    public UnityEvent<float> OnDistanceUpdated;

    Transform player;
    float nextAttackTime;

    CharacterController selfCC, playerCC;

    void Awake()
    {
        var go = GameObject.FindWithTag(playerTag);
        if (!go) Debug.LogError($"[TargetDetector] No object tagged '{playerTag}' found!", this);
        else player = go.transform;

        selfCC = GetComponent<CharacterController>();
        if (player) playerCC = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!player) return;

        
        float centerDist = ignoreVertical
            ? HorizontalDistance(transform.position, player.position)
            : Vector3.Distance(transform.position, player.position);

        
        float myR = selfCC ? selfCC.radius : 0f;
        float targetR = playerCC ? playerCC.radius : 0f;
        float edgeDist = centerDist - (myR + targetR);

        OnDistanceUpdated?.Invoke(edgeDist);

        
        if (edgeDist <= attackRange + hysteresis && Time.time >= nextAttackTime)
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
        
        var cc = GetComponent<CharacterController>();
        float myR = cc ? cc.radius : 0.5f;
        float targetR = 0.5f; 
        float approxCenter = attackRange + myR + targetR;

        Gizmos.color = Color.red;
        Vector3 c = transform.position; c.y = 0f;
        Gizmos.DrawWireSphere(c, approxCenter);
    }
#endif
}
