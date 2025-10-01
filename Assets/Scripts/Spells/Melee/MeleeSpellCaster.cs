using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float cooldown = 0.5f;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int damage = 35;
    [SerializeField] private float bounceBackStrength = 200f;

    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float shockwaveGrowSpeed = 10f;
    [SerializeField] private float blastGrowSpeed = 6f;

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
        animator.ExecuteAttack1();
        GameObject instance = Instantiate(spellPrefab, playerPosition, Quaternion.LookRotation(playerDirection));
        MeleeSpell spell = instance.GetComponent<MeleeSpell>();
        Assert.IsNotNull(spell);
        spell.blastPosition = staffPosition;
        spell.lifetime = lifetime;
        spell.damage = damage;
        spell.bounceBackStrength = bounceBackStrength;
        spell.shockwave.growSpeed = shockwaveGrowSpeed;
        spell.blast.growSpeed = blastGrowSpeed;
        spell.blast.moveSpeed = moveSpeed;
    }
}
