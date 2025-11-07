using UnityEngine;

public class BishopRangedAI : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 3f;
    public float attackRange = 8f;
    public float attackCooldown = 2f;
    public float fireballSpeed = 12f;
    public int damagePerFireball = 15;
    public GameObject fireballPrefab;
    public Transform firePoint;
    
    private CharacterController controller;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        
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
        
        if (distanceToPlayer > attackRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            
            Vector3 moveDirection = direction * moveSpeed;
            controller.Move(moveDirection * Time.deltaTime);
            
            FacePlayer();
            UpdateAnimationSpeed(moveSpeed);
        }
        else if (distanceToPlayer < stoppingDistance)
        {
            Vector3 directionAway = (transform.position - player.position).normalized;
            directionAway.y = 0;
            Vector3 moveDirection = directionAway * moveSpeed * 0.5f;
            controller.Move(moveDirection * Time.deltaTime);
            
            FacePlayer();
            UpdateAnimationSpeed(moveSpeed * 0.5f);
        }
        else
        {
            FacePlayer();
            UpdateAnimationSpeed(0f);
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

    void UpdateAnimationSpeed(float currentSpeed)
    {
        if (animator == null) return;
        
        animator.SetFloat("AnimationSpeed", currentSpeed);
        
        animator.SetBool("IsMoving", currentSpeed > 0.1f);
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

        StartCoroutine(PlayAttackAnimation());
    }

    System.Collections.IEnumerator PlayAttackAnimation()
    {
        isAttacking = true;
        animator.SetTrigger("Fire");
        
        yield return new WaitForSeconds(0.4f);
        
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Vector3 attackDirection = (player.position + Vector3.up * 0.5f) - firePoint.position;
        
        FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
        if (fireballScript != null)
        {
            fireballScript.speed = fireballSpeed;
            fireballScript.damage = damagePerFireball;
            fireballScript.Initialize(attackDirection);
        }
        
        yield return new WaitForSeconds(0.5f);
        
        isAttacking = false;
    }

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
}