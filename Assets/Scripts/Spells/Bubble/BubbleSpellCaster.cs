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
    private float cooldownLeft = 0f;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(animator);
        Assert.IsNotNull(spellPrefab);
    }

    void Update()
    {
        if (cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
    }

    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Vector3 cameraDirection, Transform player)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        animator.ExecuteAttack2();
        GameObject instance = Instantiate(spellPrefab, playerPosition, Quaternion.LookRotation(playerDirection), parent: player);
        BubbleSpell spell = instance.GetComponent<BubbleSpell>();
        Assert.IsNotNull(spell);
        spell.duration = duration;
        spell.repelRadius = repelRadius;
        spell.growDuration = growDuration;
        spell.bounceBackStrength = bounceBackStrength;
    }
}
