using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SniperSpellCaster : MonoBehaviour, ISpellCaster, ICallbackOnAttack2Climax
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] public float cooldown = 0.5f;
    [SerializeField] public float range = 30f;
    [SerializeField] public int maxEnemiesCanHit = 3;
    [SerializeField] public int damage = 20;
    [SerializeField] public float spellScale = 1f;
    [SerializeField] private float vfxExplosionRadius = 1f;
    [SerializeField] private GameObject explosionFX;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 2.5f;
    [SerializeField] private float verticalSpawnOffset = 0.3f;
    [SerializeField] public float initialSpeed = 50f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float crosshairAimingClip = 50f;

    private PlayerAnimatorController animator;
    private CrosshairsController crosshairsController;
    private SpellManager spellManager;
    private float cooldownLeft = 0f;
    private bool attacking = false;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(animator);
        crosshairsController = FindFirstObjectByType<CrosshairsController>();
        Assert.IsNotNull(crosshairsController);
        Assert.IsNotNull(spellPrefab);
    }

    void Start()
    {
        animator.RegisterOnAttack2Climax(this);
    }

    void Update()
    {
        if (!attacking && cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
    }

    public void Select()
    {
        crosshairsController.SetShowing(true);
    }

    public void CastSpell(SpellManager manager)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        attacking = true;
        spellManager = manager;
        animator.ExecuteAttack2();
    }

    public void OnAttack2Climax()
    {
        if (attacking)
        {
            attacking = false;
            // cast spell on climax
            GameObject instance = Instantiate(spellPrefab, spellManager.GetStaffTipPosition() + verticalSpawnOffset * Vector3.up,
                Quaternion.LookRotation(crosshairsController.GetWorldDirection(spellManager.GetStaffTipPosition(), maxClip: crosshairAimingClip)));
            SniperSpell spell = instance.GetComponent<SniperSpell>();
            Assert.IsNotNull(spell);
            spell.range = range;
            spell.maxEnemiesCanHit = maxEnemiesCanHit;
            spell.damage = damage;
            spell.speed = initialSpeed;
            spell.acceleration = acceleration;
            spell.transform.localScale = spellScale * Vector3.one;
            spell.vfxExplosionRadius = vfxExplosionRadius;
            spell.explosionFX = explosionFX;
        }
    }

    public float GetNormalizedCooldown()
    {
        return Mathf.Clamp01(cooldownLeft / cooldown);
    }
}
