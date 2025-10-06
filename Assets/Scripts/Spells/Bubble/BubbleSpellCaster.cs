using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BubbleSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float repelRadius = 3f;
    [SerializeField] private float bounceBackStrength = 200f;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 1.5f;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float growDuration = 0.2f;

    private PlayerAnimatorController animator;
    private CrosshairsController crosshairsController;
    private float cooldownLeft = 0f;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(animator);
        Assert.IsNotNull(animator);
        crosshairsController = FindFirstObjectByType<CrosshairsController>();
        Assert.IsNotNull(spellPrefab);
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
        animator.ExecuteAttack1();
        GameObject instance = Instantiate(spellPrefab, manager.GetPlayerPosition(), Quaternion.LookRotation(manager.GetPlayerForwardVector()), parent: manager.transform);
        BubbleSpell spell = instance.GetComponent<BubbleSpell>();
        Assert.IsNotNull(spell);
        spell.duration = duration;
        spell.repelRadius = repelRadius;
        spell.growDuration = growDuration;
        spell.bounceBackStrength = bounceBackStrength;
    }

    public float GetNormalizedCooldown()
    {
        return Mathf.Clamp01(cooldownLeft / cooldown);
    }
}
