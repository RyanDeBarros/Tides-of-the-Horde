using UnityEngine;
using UnityEngine.Assertions;

public class BombSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] public float cooldown = 0.5f;
    [SerializeField] public int innerDamage = 120;
    [SerializeField] public int outerDamage = 50;
    [SerializeField] public float bounceBackStrength = 50f;
    [SerializeField] public float aoeRadius = 1f;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private GameObject explosionFX;

    [Header("Locomotion")]
    [SerializeField] private float animationSpeedMultiplier = 3f;
    [SerializeField] public float gravity = -40f;
    [SerializeField] public float initialVerticalVelocity = 12f;
    [SerializeField] public float initialForwardVelocity = 20f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip BombSFX;

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
        Assert.IsNotNull(audioSource);
    }

    void Update()
    {
        if (cooldownLeft > 0f) cooldownLeft -= Time.deltaTime;
    }

    public void Select()
    {
        crosshairsController.SetShowing(true);
    }

    public void CastSpell(SpellManager manager)
    {
        if (cooldownLeft > 0f) return;

        // play bomb sound
        if (audioSource != null && BombSFX != null)
        {
            audioSource.PlayOneShot(BombSFX);

        }

        cooldownLeft = cooldown;
        animator.SetAttackAnimSpeed(animationSpeedMultiplier);
        animator.ExecuteAttack2();
        Vector3 direction = crosshairsController.GetWorldDirection();
        GameObject instance = Instantiate(spellPrefab, manager.GetStaffTipPosition(), Quaternion.LookRotation(direction));
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

    public float GetNormalizedCooldown()
    {
        return Mathf.Clamp01(cooldownLeft / cooldown);
    }
}
