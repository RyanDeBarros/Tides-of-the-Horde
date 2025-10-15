using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO implement strategy enum that determines how to use ranked data (ADD_INT, ADD_FLOAT, MULT_INT, MULT_FLOAT).

public class UnlockActionRankedData
{
    public int int1;
    public float float1;
}

[Serializable]
public class UnlockActionData
{
    public string id;
    public List<int> int1;
    public List<float> float1;

    public UnlockActionRankedData RankedData(int index)
    {
        return new UnlockActionRankedData()
        {
            int1 = int1 != null ? int1[index] : 0,
            float1 = float1 != null ? float1[index] : 0f
        };
    }
}

public class UnlockActionTable
{
    private enum SpellUpgradeParameter
    {
        Damage,
        Range,
        Cooldown
    }

    private readonly Dictionary<string, Action> actions = new();
    private readonly Dictionary<string, UnlockActionData> data = new();

    private SpellManager spellManager;

    public void Load(List<UnlockActionData> listData)
    {
        listData.ForEach(d => data[d.id] = d);

        SpellManager[] spellManagers = GameObject.FindObjectsByType<SpellManager>(FindObjectsSortMode.None);
        Assert.IsTrue(spellManagers.Length == 1);
        spellManager = spellManagers[0];

        LoadSpellUnlocks();
        LoadSpellUpgrades();
    }

    private void LoadSpellUnlocks()
    {
        actions["f-MeleeSpell-Unlock"] = () => UnlockSpell(SpellType.Melee);
        actions["f-BombSpell-Unlock"] = () => UnlockSpell(SpellType.Bomb);
        actions["f-BubbleSpell-Unlock"] = () => UnlockSpell(SpellType.Bubble);
        actions["f-SniperSpell-Unlock"] = () => UnlockSpell(SpellType.Sniper);
    }

    private void LoadSpellUpgrades()
    {
        void AddSpellUpgrade(SpellType spellType, SpellUpgradeParameter param, int count)
        {
            string prefix = $"f-{spellType}Spell-Upgrade-{param}";
            for (int i = 0; i < count; ++i)
            {
                int index = i;
                actions[$"{prefix}-{i + 1}"] = () => UpgradeSpell(spellType, param, data[prefix].RankedData(index));
            }
        }

        // TODO use variable length loops
        AddSpellUpgrade(SpellType.Melee, SpellUpgradeParameter.Damage, 1);
        AddSpellUpgrade(SpellType.Bomb, SpellUpgradeParameter.Damage, 1);
        AddSpellUpgrade(SpellType.Bomb, SpellUpgradeParameter.Range, 2);
    }

    public Action GetAction(string unlockID)
    {
        return actions[unlockID];
    }

    private void UnlockSpell(SpellType spell)
    {
        spellManager.UnlockSpell(spell);
    }

    private void UpgradeSpell(SpellType spell, SpellUpgradeParameter param, UnlockActionRankedData data)
    {
        ISpellCaster spellCaster = spellManager.GetSpellCaster(spell);
        switch (spell)
        {
            case SpellType.Melee:
                MeleeSpellCaster meleeSpellCaster = (MeleeSpellCaster)spellCaster;
                Assert.IsNotNull(meleeSpellCaster);
                UpgradeMeleeSpell(meleeSpellCaster, param, data);
                break;
            case SpellType.Bomb:
                BombSpellCaster bombSpellCaster = (BombSpellCaster)spellCaster;
                Assert.IsNotNull(bombSpellCaster);
                UpgradeBombSpell(bombSpellCaster, param, data);
                break;
            case SpellType.Bubble:
                BubbleSpellCaster bubbleSpellCaster = (BubbleSpellCaster)spellCaster;
                Assert.IsNotNull(bubbleSpellCaster);
                UpgradeBubbleSpell(bubbleSpellCaster, param, data);
                break;
            case SpellType.Sniper:
                SniperSpellCaster sniperSpellCaster = (SniperSpellCaster)spellCaster;
                Assert.IsNotNull(sniperSpellCaster);
                UpgradeSniperSpell(sniperSpellCaster, param, data);
                break;
        }
    }

    private void UpgradeMeleeSpell(MeleeSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, UnlockActionRankedData data)
    {
        // TODO
    }

    private void UpgradeBombSpell(BombSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, UnlockActionRankedData data)
    {
        // TODO
    }

    private void UpgradeBubbleSpell(BubbleSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, UnlockActionRankedData data)
    {
        // TODO
    }

    private void UpgradeSniperSpell(SniperSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, UnlockActionRankedData data)
    {
        // TODO
    }
}
