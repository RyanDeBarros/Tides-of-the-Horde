using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private Transform staffTip;
    [SerializeField] private SpellType activeSpell = SpellType.Melee;

    [Header("UI")]
    [SerializeField] private HUDController hud;
    [SerializeField] private Texture meleeSpellIcon;
    [SerializeField] private Texture bombSpellIcon;
    [SerializeField] private Texture bubbleSpellIcon;
    [SerializeField] private Texture sniperSpellIcon;

    private static readonly int LMB = 0;
    private readonly Dictionary<SpellType, UnlockableSpellCaster> spellCasters = new();
    private PlayerCamera cam;

    private void Awake()
    {
        spellCasters[SpellType.Melee] = new UnlockableSpellCaster(GetComponentInChildren<MeleeSpellCaster>());
        spellCasters[SpellType.Bomb] = new UnlockableSpellCaster(GetComponentInChildren<BombSpellCaster>());
        spellCasters[SpellType.Bubble] = new UnlockableSpellCaster(GetComponentInChildren<BubbleSpellCaster>());
        spellCasters[SpellType.Sniper] = new UnlockableSpellCaster(GetComponentInChildren<SniperSpellCaster>());

        cam = GetComponent<PlayerCamera>();
        Assert.IsNotNull(cam);

        Assert.IsNotNull(body);
        Assert.IsNotNull(staffTip);

        Assert.IsNotNull(hud);
        Assert.IsNotNull(meleeSpellIcon);
        Assert.IsNotNull(bombSpellIcon);
        Assert.IsNotNull(bubbleSpellIcon);
        Assert.IsNotNull(sniperSpellIcon);
    }

    private void Start()
    {
        UnlockSpell(activeSpell);
        GetActiveSpellCaster().Select();
        hud.GetSpells().ForEach(spell => {
            if (spell.GetSpellType() == activeSpell)
                spell.ShowSelected();
            else
                spell.ShowDeselected();
        });
    }

    void Update()
    {
        CheckForToggleInput();

        if (Input.GetMouseButtonDown(LMB))
            GetActiveSpellCaster().CastSpell(this);

        hud.GetSpells().ForEach(spell => {
            if (spellCasters.TryGetValue(spell.GetSpellType(), out UnlockableSpellCaster caster))
                spell.SetCooldown(caster.caster.GetNormalizedCooldown());
        });
    }

    private void CheckForToggleInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
            ToggleActiveSpellUp();
        else if (scroll < 0)
            ToggleActiveSpellDown();
        else
        {
            for (int i = 0; i < Math.Min(9, hud.GetSpells().Count); ++i)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + (i + 1) % 10))
                {
                    SpellType spellType = hud.GetSpells()[i].GetSpellType();
                    if (!spellCasters[spellType].locked)
                        SetActiveSpell(spellType);
                    break;
                }
            }
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
        if (activeSpell == spellType)
            return;

        Assert.IsTrue(IsUnlocked(spellType));
        activeSpell = spellType;
        GetActiveSpellCaster().Select();

        hud.GetSpells().ForEach(spell => {
            if (spell.GetSpellType() == activeSpell)
                spell.ShowSelected();
            else
                spell.ShowDeselected();
        });
    }

    private ISpellCaster GetActiveSpellCaster()
    {
        return GetSpellCaster(activeSpell);
    }

    public ISpellCaster GetSpellCaster(SpellType spellType)
    {
        return spellCasters[spellType].caster;
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
        if (IsUnlocked(spellType))
            return;

        spellCasters[spellType].locked = false;
        var spellUI = hud.GetSpells().Where(spell => spell.GetSpellType() == spellType);
        Assert.IsTrue(spellUI.Any());
        spellUI.First().ShowUnlocked();
    }

    public void LockSpell(SpellType spellType)
    {
        if (!IsUnlocked(spellType))
            return;

        spellCasters[spellType].locked = true;
        var spellUI = hud.GetSpells().Where(spell => spell.GetSpellType() == spellType);
        Assert.IsTrue(spellUI.Any());
        spellUI.First().ShowLocked();
    }

    public bool IsUnlocked(SpellType spellType)
    {
        return !spellCasters[spellType].locked;
    }
}
