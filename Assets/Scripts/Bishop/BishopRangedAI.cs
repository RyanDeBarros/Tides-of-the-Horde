using UnityEngine;

public class BishopRangedAI : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;
    
    [Header("Movement Settings")]
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 3f; // Reduced for better backing away
    
    [Header("Attack Settings")]
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public float fireballSpeed = 12f;
    public int damagePerFireball = 15;
    
    [Header("References")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    
    private CharacterController controller;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
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
        UpdateAnimations();
    }

    void HandleMovement()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Move towards player if outside attack range
        if (distanceToPlayer > attackRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            
            Vector3 moveDirection = direction * moveSpeed;
            controller.Move(moveDirection * Time.deltaTime);
            
            FacePlayer();
        }
        // Back away if too close
        else if (distanceToPlayer < stoppingDistance)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;
            directionAway.y = 0;
            Vector3 moveDirection = directionAway * moveSpeed * 0.5f;
            controller.Move(moveDirection * Time.deltaTime);
            
            FacePlayer(); // Still face player while backing away!
        }
        // In perfect position - just face player
        else
        {
            FacePlayer();
        }
    }

    void HandleAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = (distanceToPlayer <= attackRange && distanceToPlayer >= stoppingDistance);

        if (playerInRange && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        // Calculate movement speed for walk/idle animations
        float horizontalSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        animator.SetFloat("Speed", horizontalSpeed);
        
        // Handle attack animation
        animator.SetBool("IsAttacking", isAttacking);
    }

    void FacePlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

    void Attack()
    {
        if (fireballPrefab == null) return;

        // Trigger attack animation
        StartCoroutine(PlayAttackAnimation());
    }

    System.Collections.IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;
        
        // Wait for attack animation to reach the point where fireball should spawn
        yield return new WaitForSeconds(0.3f); // Adjust this timing to match your animation
        
        // Spawn fireball during attack animation
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Vector3 attackDirection = (player.position + Vector3.up * 0.5f) - firePoint.position;
        
        FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
        if (fireballScript != null)
        {
            fireballScript.speed = fireballSpeed;
            fireballScript.damage = damagePerFireball;
            fireballScript.Initialize(attackDirection);
        }
        
        // Wait for attack animation to finish
        yield return new WaitForSeconds(0.5f); // Adjust to match your animation length
        
        isAttacking = false;
    }

    // Animation Event methods - call these from your attack animation if using animation events
    public void OnAttackSpawnFireball()
    {
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

    public void OnAttackEnd()
    {
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}