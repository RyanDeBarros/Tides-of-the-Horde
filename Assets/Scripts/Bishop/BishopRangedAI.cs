using UnityEngine;

public class BishopRangedAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 5f;
    
    [Header("Attack Settings")]
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public float fireballSpeed = 12f;
    public int damagePerFireball = 15;
    
    [Header("References")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    
    private CharacterController controller;
    private float lastAttackTime;
    private bool playerInRange = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0, 1f, 1f);
        }
    }

    void Update()
    {
        if (player == null) return;

        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
{
    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    
    // Move towards player if outside attack range
    if (distanceToPlayer > attackRange)
    {
        // Calculate direction to player
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        // Move toward player
        Vector3 moveDirection = direction * moveSpeed;
        controller.Move(moveDirection * Time.deltaTime);
        
        // Face the player
        FacePlayer();
    }
    // If we're in attack range but too close, back away slightly
    else if (distanceToPlayer < stoppingDistance)
    {
        Vector3 directionAway = (transform.position - player.position).normalized;
        directionAway.y = 0;
        Vector3 moveDirection = directionAway * moveSpeed * 0.5f;
        controller.Move(moveDirection * Time.deltaTime);
        
        // IMPORTANT: Still face the player while backing away!
        FacePlayer();
    }
    // Otherwise, we're in perfect attack position - don't move but face player
    else
    {
        FacePlayer();
    }
}

void FacePlayer()
{
    if (player == null) return;
    
    Vector3 lookPos = player.position - transform.position;
    lookPos.y = 0;
    if (lookPos != Vector3.zero)
    {
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}

    void HandleAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        playerInRange = (distanceToPlayer <= attackRange && distanceToPlayer >= stoppingDistance);

        if (playerInRange)
        {
            FacePlayer();
            
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
    }

    void Attack()
    {
        if (fireballPrefab == null) return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Vector3 attackDirection = (player.position + Vector3.up * 0.5f) - firePoint.position;
        
        FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
        if (fireballScript != null)
        {
            fireballScript.speed = fireballSpeed;
            fireballScript.damage = damagePerFireball;
            fireballScript.Initialize(attackDirection);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}