using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SniperSpellCaster : MonoBehaviour, ISpellCaster, ICallbackOnAttack2Climax
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
    private CrosshairsController crosshairsController;
    private float cooldownLeft = 0f;
    private bool attacking = false;

    private class SpellCastSetup
    {
        public Vector3 staffPosition;
        public Vector3 direction;
    }

    private SpellCastSetup spellCastSetup;

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

    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Transform player)
    {
        if (cooldownLeft > 0f) return;

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        attacking = true;
        // delay casting spell
        spellCastSetup = new()
        {
            staffPosition = staffPosition,
            direction = crosshairsController.GetWorldDirection()
        };
        animator.ExecuteAttack2();
    }

    public void OnAttack2Climax()
    {
        if (attacking)
        {
            attacking = false;
            // cast spell on climax
            GameObject instance = Instantiate(spellPrefab, spellCastSetup.staffPosition + verticalSpawnOffset * Vector3.up, Quaternion.LookRotation(spellCastSetup.direction));
            SniperSpell spell = instance.GetComponent<SniperSpell>();
            Assert.IsNotNull(spell);
            spell.lifespan = lifespan;
            spell.damage = damage;
            spell.speed = initialSpeed;
            spell.acceleration = acceleration;
        }
    }
}
