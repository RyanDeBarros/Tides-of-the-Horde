using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class BishopRangedAI : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackAudioClip;

    public Transform player;
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 3f;
    [SerializeField] private float turnSpeed = 600f;

    public float attackRange = 8f;
    public float attackCooldown = 2f;

    public float fireballSpeed = 12f;
    public int damagePerFireball = 15;
    public GameObject fireballPrefab;
    public Transform firePoint;

    private CharacterController controller;
    private NavMover mover;
    private Animator animator;
    private float lastAttackTime;
    private bool isAttacking = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);

        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        mover = GetComponent<NavMover>();
        Assert.IsNotNull(mover);

        animator = GetComponentInChildren<Animator>();
        Assert.IsNotNull(animator);

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);
        
        Assert.IsNotNull(fireballPrefab);
        
        if (firePoint == null)
        {
            firePoint = new GameObject("FirePoint").transform;
            firePoint.SetParent(transform);
            firePoint.localPosition = new Vector3(0, 1f, 1f);
        }
        Assert.IsNotNull(firePoint);
    }

    private void Update()
    {
        UpdateMovement();
        UpdateAttack();
    }

    private void UpdateMovement()
    {
        Vector3 movement = mover.MoveController(player.position - transform.position, stoppingDistance, moveSpeed, turnSpeed);
        animator.SetBool("IsMoving", movement.magnitude > 0.01f);
    }

    private void UpdateAttack()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool playerInRange = (distanceToPlayer <= attackRange && distanceToPlayer >= stoppingDistance);

        if (playerInRange && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    private void Attack()
    {
        StartCoroutine(PlayAttackAnimation());
    }

    private IEnumerator PlayAttackAnimation()
    {
        if (attackAudioClip != null)
            audioSource.PlayOneShot(attackAudioClip);

        isAttacking = true;
        animator.SetTrigger("Fire");
        yield return new WaitForSeconds(0.4f);
        OnAttackSpawnFireball();
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void OnAttackSpawnFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, firePoint.rotation);
        Vector3 attackDirection = (player.position + Vector3.up * 0.5f) - firePoint.position;
        
        FireballProjectile fireballScript = fireball.GetComponent<FireballProjectile>();
        Assert.IsNotNull(fireballScript);
        fireballScript.speed = fireballSpeed;
        fireballScript.damage = damagePerFireball;
        fireballScript.Initialize(attackDirection);
    }

    public void OnAttackEnd()
    {
        isAttacking = false;
    }
}
