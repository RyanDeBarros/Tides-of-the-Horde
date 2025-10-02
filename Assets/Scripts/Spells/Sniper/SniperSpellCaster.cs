using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SniperSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float cooldown = 0.5f;
    [SerializeField] private float lifespan = 3f;
    [SerializeField] private int damage = 20;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 2.5f;
    [SerializeField] private float verticalSpawnOffset = 0.3f;
    [SerializeField] private float initialSpeed = 50f;
    [SerializeField] private float acceleration = 50f;

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
        // TODO wait until animation finishes
        cameraDirection.y = 0f;
        cameraDirection.Normalize();
        GameObject instance = Instantiate(spellPrefab, staffPosition + verticalSpawnOffset * Vector3.up, Quaternion.LookRotation(cameraDirection));
        SniperSpell spell = instance.GetComponent<SniperSpell>();
        Assert.IsNotNull(spell);
        spell.lifespan = lifespan;
        spell.damage = damage;
        spell.speed = initialSpeed;
        spell.acceleration = acceleration;
    }
}
