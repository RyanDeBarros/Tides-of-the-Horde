using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BombSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private int innerDamage = 120;
    [SerializeField] private int outerDamage = 50;
    [SerializeField] private float bounceBackStrength = 50f;
    [SerializeField] private float aoeRadius = 1f;
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 3f;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float initialVerticalVelocity = 5f;
    [SerializeField] private float initialForwardVelocity = 10f;

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

    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Vector3 cameraDirection)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        animator.ExecuteAttack2();
        GameObject instance = Instantiate(spellPrefab, staffPosition, Quaternion.LookRotation(cameraDirection));
        BombSpell spell = instance.GetComponent<BombSpell>();
        Assert.IsNotNull(spell);
        spell.innerDamage = innerDamage;
        spell.outerDamage = outerDamage;
        spell.bounceBackStrength = bounceBackStrength;
        spell.aoeRadius = aoeRadius;
        spell.enemyLayerMask = enemyLayerMask;
        spell.gravity = gravity;
        spell.velocity = cameraDirection * initialForwardVelocity + Vector3.up * initialVerticalVelocity;
    }
}
