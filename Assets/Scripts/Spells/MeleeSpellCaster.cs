using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpellCaster : MonoBehaviour, ISpellCaster
{
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private float lifetime = 0.3f;
    [SerializeField] private float shockwaveGrowSpeed = 20f;
    [SerializeField] private float blastGrowSpeed = 10f;
    [SerializeField] private float moveSpeed = 10f;

    private PlayerAnimatorController animator;

    void Awake()
    {
        animator = transform.parent.GetComponentInChildren<PlayerAnimatorController>();
    }

    void Start()
    {
        Assert.IsNotNull(animator);
        Assert.IsNotNull(spellPrefab);
    }

    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Vector3 cameraDirection)
    {
        animator.ExecuteAttack1();
        GameObject instance = Instantiate(spellPrefab, playerPosition, Quaternion.LookRotation(playerDirection));
        MeleeSpell spell = instance.GetComponent<MeleeSpell>();
        Assert.IsNotNull(spell);
        spell.blastPosition = staffPosition;
        spell.lifetime = lifetime;
        spell.shockwaveGrowSpeed = shockwaveGrowSpeed;
        spell.blastGrowSpeed = blastGrowSpeed;
        spell.moveSpeed = moveSpeed;
    }
}
