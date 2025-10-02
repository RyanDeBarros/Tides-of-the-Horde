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
    [SerializeField] private float initialVerticalVelocity = 10f;
    [SerializeField] private float initialForwardVelocity = 20f;

    private PlayerAnimatorController animator;
    private CrosshairsController crosshairsController;
    private float cooldownLeft = 0f;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
        Assert.IsNotNull(animator);
        crosshairsController = FindFirstObjectByType<CrosshairsController>();
        Assert.IsNotNull(crosshairsController);
        Assert.IsNotNull(spellPrefab);
    }

    void Update()
    {
        if (cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
    }

    public void Select()
    {
        crosshairsController.SetShowing(true);
    }

    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Transform player)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        animator.ExecuteAttack2();
        Vector3 direction = crosshairsController.GetWorldDirection();
        GameObject instance = Instantiate(spellPrefab, staffPosition, Quaternion.LookRotation(direction));
        BombSpell spell = instance.GetComponent<BombSpell>();
        Assert.IsNotNull(spell);
        spell.innerDamage = innerDamage;
        spell.outerDamage = outerDamage;
        spell.bounceBackStrength = bounceBackStrength;
        spell.aoeRadius = aoeRadius;
        spell.enemyLayerMask = enemyLayerMask;
        spell.gravity = gravity;
        spell.velocity = direction * initialForwardVelocity + Vector3.up * initialVerticalVelocity;
    }
}
