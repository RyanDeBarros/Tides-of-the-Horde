using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SpellManager : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private Transform staffTip;
    [SerializeField] private SpellType activeSpell = SpellType.Sniper;

    private Dictionary<SpellType, ISpellCaster> spellCasters = new();

    private PlayerCamera cam;

    private void Awake()
    {
        spellCasters[SpellType.Melee] = GetComponentInChildren<MeleeSpellCaster>();
        spellCasters[SpellType.Bomb] = GetComponentInChildren<BombSpellCaster>();
        spellCasters[SpellType.Bubble] = GetComponentInChildren<BubbleSpellCaster>();
        spellCasters[SpellType.Sniper] = GetComponentInChildren<SniperSpellCaster>();
        foreach (ISpellCaster spellCaster in spellCasters.Values)
            Assert.IsNotNull(spellCaster);

        cam = GetComponent<PlayerCamera>();
        Assert.IsNotNull(cam);

        Assert.IsNotNull(body);
        Assert.IsNotNull(staffTip);

        SetActiveSpell(activeSpell);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 = left click
        {
            GetActiveSpellCaster().CastSpell(body.transform.position, staffTip.position, body.transform.forward, transform);
        }
    }

    public void SetActiveSpell(SpellType spellType)
    {
        activeSpell = spellType;
        GetActiveSpellCaster().Select();
    }

    private ISpellCaster GetActiveSpellCaster()
    {
        return spellCasters[activeSpell];
    }
}
