using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private Transform staffTip;
    [SerializeField] private SpellType activeSpell = SpellType.Sniper;

    private readonly Dictionary<SpellType, UnlockableSpellCaster> spellCasters = new();

    private PlayerCamera cam;

    private void Awake()
    {
        spellCasters[SpellType.Melee] = new UnlockableSpellCaster(GetComponentInChildren<MeleeSpellCaster>());
        spellCasters[SpellType.Bomb] = new UnlockableSpellCaster(GetComponentInChildren<BombSpellCaster>());
        spellCasters[SpellType.Bubble] = new UnlockableSpellCaster(GetComponentInChildren<BubbleSpellCaster>());
        spellCasters[SpellType.Sniper] = new UnlockableSpellCaster(GetComponentInChildren<SniperSpellCaster>());

        UnlockSpell(SpellType.Melee);
        UnlockSpell(SpellType.Bomb);
        UnlockSpell(SpellType.Bubble);
        UnlockSpell(SpellType.Sniper);

        cam = GetComponent<PlayerCamera>();
        Assert.IsNotNull(cam);

        Assert.IsNotNull(body);
        Assert.IsNotNull(staffTip);
    }

    private void Start()
    {
        SetActiveSpell(activeSpell);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            GetActiveSpellCaster().CastSpell(this);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return body.transform.position;
    }

    public Vector3 GetPlayerForwardVector()
    {
        return body.transform.forward.normalized;
    }

    public Vector3 GetStaffTipPosition()
    {
        return staffTip.position;
    }

    public void SetActiveSpell(SpellType spellType)
    {
        Assert.IsTrue(IsUnlocked(spellType));
        activeSpell = spellType;
        GetActiveSpellCaster().Select();
    }

    private ISpellCaster GetActiveSpellCaster()
    {
        return spellCasters[activeSpell].caster;
    }

    public void ToggleActiveSpellUp()
    {
        ToggleActiveSpell(up: true);
    }

    public void ToggleActiveSpellDown()
    {
        ToggleActiveSpell(up: false);
    }

    private void ToggleActiveSpell(bool up)
    {
        SpellType[] spellTypes = (SpellType[])Enum.GetValues(typeof(SpellType));
        int activeIndex = Array.IndexOf(spellTypes, activeSpell);
        for (int offset = 1; offset < spellTypes.Length; ++offset)
        {
            SpellType nextSpell = spellTypes[(activeIndex + offset * (up ? 1 : -1) + spellTypes.Length) % spellTypes.Length];
            if (IsUnlocked(nextSpell))
            {
                SetActiveSpell(nextSpell);
                break;
            }
        }
    }

    public void UnlockSpell(SpellType spellType)
    {
        spellCasters[spellType].locked = false;
    }

    public void LockSpell(SpellType spellType)
    {
        spellCasters[spellType].locked = true;
    }

    public bool IsUnlocked(SpellType spellType)
    {
        return !spellCasters[spellType].locked;
    }
}
