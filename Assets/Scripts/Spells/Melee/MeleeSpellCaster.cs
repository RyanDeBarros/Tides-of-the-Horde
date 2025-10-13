using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpellCaster : MonoBehaviour, ISpellCaster, ICallbackOnAttack1Climax
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private int damage = 35;
    [SerializeField] private float bounceBackStrength = 200f;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 2f;
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float shockwaveGrowSpeed = 5f;
    [SerializeField] private float blastGrowSpeed = 6f;
    [SerializeField] private float shockwaveForwardOffset = 1f;

    private PlayerAnimatorController animator;
    private CrosshairsController crosshairsController;
    private SpellManager spellManager;
    private float cooldownLeft = 0f;
    private bool attacking = false;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(animator);
        Assert.IsNotNull(animator);
        crosshairsController = FindFirstObjectByType<CrosshairsController>();
        Assert.IsNotNull(spellPrefab);
    }

    void Start()
    {
        animator.RegisterOnAttack1Climax(this);
    }

    void Update()
    {
        if (cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
    }

    public void Select()
    {
        crosshairsController.SetShowing(false);
    }

    public void CastSpell(SpellManager manager)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        attacking = true;
        spellManager = manager;
        animator.ExecuteAttack1();
    }

    public void OnAttack1Climax()
    {
        if (attacking)
        {
            attacking = false;
            // cast spell on climax
            GameObject instance = Instantiate(spellPrefab, spellManager.GetPlayerPosition() + spellManager.GetPlayerForwardVector() * shockwaveForwardOffset,
                Quaternion.LookRotation(spellManager.GetPlayerForwardVector()));
            MeleeSpell spell = instance.GetComponent<MeleeSpell>();
            Assert.IsNotNull(spell);
            spell.blastPosition = spellManager.GetStaffTipPosition();
            spell.lifetime = lifetime;
            spell.damage = damage;
            spell.bounceBackStrength = bounceBackStrength;
            spell.shockwave.growSpeed = shockwaveGrowSpeed;
            spell.blast.growSpeed = blastGrowSpeed;
            spell.blast.moveSpeed = moveSpeed;
        }
    }

    public float GetNormalizedCooldown()
    {
        return Mathf.Clamp01(cooldownLeft / cooldown);
    }
}
