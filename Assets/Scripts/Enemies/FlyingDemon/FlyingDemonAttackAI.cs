using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class FlyingDemonAttackAI : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AudioSource audioSource;

    [Header("Punch")]
    public int punchDamage = 5;
    public float punchBounceBackStrength = 120f;
    public float punchProbabilityWeight = 1.0f;
    public float punchAttackRange = 5f;
    [SerializeField] private List<SphereCollider> punchColliders;
    [SerializeField] private AudioClip punchAudioClip;
    [SerializeField] private float punchAudioDelay = 0.5f;

    [Header("Bite")]
    public int biteDamage = 10;
    public float biteBounceBackStrength = 100f;
    public float biteProbabilityWeight = 0.75f;
    public float biteAttackRange = 5f;
    [SerializeField] private List<SphereCollider> bigBiteColliders;
    [SerializeField] private AudioClip biteAudioClip;

    [Header("Charge")]
    public int chargeDamage = 8;
    public float chargeBounceBackStrength = 100f;
    public float chargeProbabilityWeight = 0.3f;
    public float chargeAttackRange = 8f;
    public float chargeBackDistance = 3.5f;
    public float chargeBackSpeed = 8f;
    public float chargeForwardSpeed = 20f;
    public float dizzyDuration = 3f;
    [SerializeField] private List<SphereCollider> smallBiteColliders;
    [SerializeField] private AudioClip chargeAudioClip;

    [Header("Cooldowns")]
    public float minCooldown = 1.0f;
    public float maxCooldown = 2.0f;

    private FlyingDemonAnimator animator;
    private FlyingDemonMovement movement;
    private CharacterController controller;
    private Transform player;
    private Health playerHealth;
    private BounceBack playerBounceBack;

    private enum AttackState
    {
        Punch,
        Bite,
        ChargeBackward,
        ChargeForward,
        ChargeBite,
        ChargeDizzy,
        Cooldown
    }

    private AttackState attackState = AttackState.Cooldown;

    private float cooldown = 0f;
    private float timeElapsed = 0f;
    private float chargeTimeLeft = 0f;

    private bool playerWasHit = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        Assert.IsNotNull(audioSource);

        animator = GetComponentInChildren<FlyingDemonAnimator>();
        Assert.IsNotNull(animator);

        movement = GetComponent<FlyingDemonMovement>();
        Assert.IsNotNull(movement);

        controller = GetComponent<CharacterController>();
        Assert.IsNotNull(controller);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(player);

        playerHealth = player.GetComponent<Health>();
        Assert.IsNotNull(playerHealth);

        playerBounceBack = player.GetComponent<BounceBack>();
        Assert.IsNotNull(playerBounceBack);
    }

    private void Update()
    {
        if (attackState == AttackState.Cooldown)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > cooldown && movement.CanAttack())
                ChooseNextAttack();
        }

        switch (attackState)
        {
            case AttackState.Punch:
                UpdatePunch();
                break;
            case AttackState.Bite:
                UpdateBite();
                break;
            case AttackState.ChargeBackward:
                UpdateChargeBackward();
                break;
            case AttackState.ChargeForward:
                UpdateChargeForward();
                break;
            case AttackState.ChargeDizzy:
                timeElapsed += Time.deltaTime;
                if (timeElapsed < dizzyDuration)
                    animator.SetDizzy();
                else
                {
                    attackState = AttackState.ChargeBite;
                    OnAttackEnd();
                }
                break;
            case AttackState.ChargeBite:
                UpdateChargeBite();
                break;
        }

        movement.LockY();
    }

    private bool AnyHits(List<SphereCollider> colliders)
    {
        return colliders.Any(c => {
            Collider[] results = new Collider[1];
            return Physics.OverlapSphereNonAlloc(c.transform.TransformPoint(c.center), c.radius, results, playerLayer) > 0;
        });
    }

    private void UpdatePunch()
    {
        if (playerWasHit) return;

        if (AnyHits(punchColliders))
        {
            playerWasHit = true;
            playerHealth.TakeDamage(punchDamage);
            BouncePlayer(punchBounceBackStrength);
        }
    }

    private void UpdateBite()
    {
        if (playerWasHit) return;

        if (AnyHits(bigBiteColliders))
        {
            playerWasHit = true;
            playerHealth.TakeDamage(biteDamage);
            BouncePlayer(biteBounceBackStrength);
        }
    }

    private void UpdateChargeBackward()
    {
        timeElapsed += Time.deltaTime;
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        movement.FacePlayer();
        if (timeElapsed < chargeTimeLeft)
        {
            controller.Move(chargeBackSpeed * Time.deltaTime * -direction.normalized);
            animator.SetMovingBackward();
        }
        else
        {
            attackState = AttackState.ChargeForward;
            timeElapsed = 0f;
            chargeTimeLeft = Mathf.Max(direction.magnitude - 0.5f * movement.stoppingDistance, 0f) / chargeForwardSpeed;
        }
    }

    private void UpdateChargeForward()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed < chargeTimeLeft)
        {
            controller.Move(chargeForwardSpeed * Time.deltaTime * transform.forward);
            animator.SetRunning();
        }
        else
        {
            attackState = AttackState.ChargeBite;
            controller.enabled = false;
            animator.SmallBiteAttack();
        }
    }

    private void UpdateChargeBite()
    {
        if (AnyHits(smallBiteColliders))
        {
            if (playerHealth.IsInvulnerable())
            {
                Stun();
                controller.enabled = true;
            }
            else if (!playerWasHit)
            {
                playerHealth.TakeDamage(biteDamage);
                BouncePlayer(chargeBounceBackStrength);
            }
            playerWasHit = true;
        }
    }

    private void BouncePlayer(float strength)
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        playerBounceBack.Bounce(direction.normalized, strength);
    }

    private void ChooseNextAttack()
    {
        timeElapsed = 0f;
        playerWasHit = false;

        float distance = Vector3.Distance(transform.position, player.position);
        float totalWeight = 0f;
        if (distance <= punchAttackRange) totalWeight += punchProbabilityWeight;
        if (distance <= biteAttackRange) totalWeight += biteProbabilityWeight;
        if (distance <= chargeAttackRange) totalWeight += chargeProbabilityWeight;
        if (totalWeight == 0f) return;

        float r = Random.value * totalWeight;
        if (distance <= punchAttackRange)
        {
            if (r < punchProbabilityWeight)
                StartPunchAttack();
            else
            {
                if (distance > biteAttackRange)
                    StartChargeAttack();
                else if (distance > chargeAttackRange)
                    StartBiteAttack();
                else
                {
                    r -= punchProbabilityWeight;
                    if (r < biteProbabilityWeight)
                        StartBiteAttack();
                    else
                        StartChargeAttack();
                }
            }
        }
        else
        {
            if (distance > biteAttackRange)
                StartChargeAttack();
            else if (distance > chargeAttackRange)
                StartBiteAttack();
            else
            {
                if (r < biteProbabilityWeight)
                    StartBiteAttack();
                else
                    StartChargeAttack();
            }
        }
    }

    private void StartPunchAttack()
    {
        attackState = AttackState.Punch;
        animator.PunchAttack();
        PlaySFX(punchAudioClip, punchAudioDelay);
    }

    private void StartBiteAttack()
    {
        attackState = AttackState.Bite;
        animator.BigBiteAttack();
        PlaySFX(biteAudioClip);
    }

    private void StartChargeAttack()
    {
        attackState = AttackState.ChargeBackward;
        chargeTimeLeft = chargeBackDistance / chargeBackSpeed;
        PlaySFX(chargeAudioClip);
    }

    private void PlaySFX(AudioClip audioClip, float delay = 0f)
    {
        IEnumerator Routine()
        {
            yield return new WaitForSeconds(delay);
            audioSource.PlayOneShot(audioClip);
        }
        
        if (audioClip != null)
            StartCoroutine(Routine());
    }

    public bool IsAttacking()
    {
        return attackState != AttackState.Cooldown;
    }

    public bool AllowMovement()
    {
        return attackState != AttackState.ChargeBackward && attackState != AttackState.ChargeForward
            && attackState != AttackState.ChargeDizzy && attackState != AttackState.ChargeBite;
    }

    public void OnAttackEnd()
    {
        if (attackState == AttackState.ChargeDizzy) return;

        timeElapsed = 0f;
        cooldown = Random.Range(minCooldown, maxCooldown);
        attackState = AttackState.Cooldown;
        controller.enabled = true;
    }

    public void Stun()
    {
        attackState = AttackState.ChargeDizzy;
        timeElapsed = 0f;
        animator.PlayGetHit();
        animator.SetDizzy();
    }
}
