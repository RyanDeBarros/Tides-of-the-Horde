using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class OrcMovementAI : MonoBehaviour
{
    [Header("Targets & Ranges")]
    public Transform player;
    public float chaseRange = 30f;          
    public float stoppingDistance = 1.5f;     

    [Header("Movement")]
    public float moveSpeed = 4f;            
    public float turnSpeed = 12f;           

    CharacterController cc;
    CharacterController playerCC;           
    float gravityY;                         

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (!player)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
        }
        if (player) playerCC = player.GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!player || !cc) return;

        
        Vector3 to = player.position - transform.position;
        Vector3 toXZ = new Vector3(to.x, 0f, to.z);
        float centerDist = toXZ.magnitude;

        
        if (centerDist > chaseRange)
        {
            ApplyGravityOnly();
            return;
        }

        
        if (toXZ.sqrMagnitude > 0.0001f)
        {
            Quaternion face = Quaternion.LookRotation(toXZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, face, turnSpeed * Time.deltaTime);
        }

        
        float myR = cc.radius;
        float targetR = playerCC ? playerCC.radius : 0f;
        float edgeDist = centerDist - (myR + targetR);

        if (edgeDist > stoppingDistance)
        {
            
            Vector3 dir = toXZ.normalized;
            Vector3 vel = dir * moveSpeed;   
            
            gravityY += Physics.gravity.y * Time.deltaTime;
            vel.y = gravityY;

            cc.Move(vel * Time.deltaTime);
        }
        else
        {
            
            ApplyGravityOnly();
        }
    }

    void ApplyGravityOnly()
    {
        gravityY += Physics.gravity.y * Time.deltaTime;
        cc.Move(new Vector3(0f, gravityY, 0f) * Time.deltaTime);
        
        if (cc.isGrounded && gravityY < 0f) gravityY = -0.5f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        if (!cc) cc = GetComponent<CharacterController>();
        float approxCenterStop = stoppingDistance + (cc ? cc.radius : 0.5f) + 0.5f; 
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, approxCenterStop);
    }
}
