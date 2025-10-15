using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum SpellUpgradeParameter
{
    Damage,
    Range,
    Cooldown,
    BounceBack,
    Radius,
    Speed,
    MaxEnemiesHit
}

[Serializable]
public class UnlockActionData
{
    [Serializable]
    private class StatsTable
    {
        public string param;
        public List<float> values;

        public SpellUpgradeParameter GetParam()
        {
            return Enum.Parse<SpellUpgradeParameter>(param);
        }
    }

    public string id;
    [SerializeField] private List<StatsTable> stats;

    public List<float> GetParamValues(SpellUpgradeParameter param)
    {
        return GetStats(param).values;
    }

    private StatsTable GetStats(SpellUpgradeParameter param)
    {
        return stats.Where(stat => stat.GetParam() == param).First();
    }

    public List<SpellUpgradeParameter> GetSupportedParameters()
    {
        return stats.Select(stat => stat.GetParam()).ToList();
    }
}

public class UnlockActionTable
{
    private readonly Dictionary<string, Action> actions = new();
    private readonly Dictionary<string, UnlockActionData> data = new();

    private SpellManager spellManager;

    public void Load(List<UnlockActionData> listData)
    {
        listData.ForEach(d => data[d.id] = d);

        spellManager = UnityEngine.Object.FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();

        LoadSpellUnlocks();
        LoadSpellUpgrades();
       // TODO health upgrade
    }

    private void LoadSpellUnlocks()
    {
        foreach (SpellType spellType in Enum.GetValues(typeof(SpellType)))
            actions[$"f-{spellType}Spell-Unlock"] = () => UnlockSpell(spellType);
    }

    private void LoadSpellUpgrades()
    {
        foreach (SpellType spellType in Enum.GetValues(typeof(SpellType)))
        {
            string prefix = $"f-{spellType}Spell-Upgrade";
            if (!data.ContainsKey(prefix)) continue;

            UnlockActionData d = data[prefix];
            d.GetSupportedParameters().ForEach(param => {
                List<float> values = d.GetParamValues(param);
                values.Select((value, i) => (value, i)).ToList().ForEach(x => {
                    actions[$"{prefix}-{param}-{x.i + 1}"] = () => UpgradeSpell(spellType, param, x.value);
                });
            });
        }
    }

    public Action GetAction(string unlockID)
    {
        return actions[unlockID];
    }

    private void UnlockSpell(SpellType spell)
    {
        spellManager.UnlockSpell(spell);
    }

    private void UpgradeSpell(SpellType spell, SpellUpgradeParameter param, float value)
    {
        ISpellCaster spellCaster = spellManager.GetSpellCaster(spell);
        switch (spell)
        {
            case SpellType.Melee:
                MeleeSpellCaster meleeSpellCaster = (MeleeSpellCaster)spellCaster;
                Assert.IsNotNull(meleeSpellCaster);
                UpgradeMeleeSpell(meleeSpellCaster, param, value);
                break;
            case SpellType.Bomb:
                BombSpellCaster bombSpellCaster = (BombSpellCaster)spellCaster;
                Assert.IsNotNull(bombSpellCaster);
                UpgradeBombSpell(bombSpellCaster, param, value);
                break;
            case SpellType.Bubble:
                BubbleSpellCaster bubbleSpellCaster = (BubbleSpellCaster)spellCaster;
                Assert.IsNotNull(bubbleSpellCaster);
                UpgradeBubbleSpell(bubbleSpellCaster, param, value);
                break;
            case SpellType.Sniper:
                SniperSpellCaster sniperSpellCaster = (SniperSpellCaster)spellCaster;
                Assert.IsNotNull(sniperSpellCaster);
                UpgradeSniperSpell(sniperSpellCaster, param, value);
                break;
        }
    }

    private void UpgradeMeleeSpell(MeleeSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, float value)
    {
        switch (spellUpgradeParameter)
        {
            case SpellUpgradeParameter.Damage:
                spellCaster.damage = (int)(spellCaster.damage * value);
                break;
            case SpellUpgradeParameter.Range:
                spellCaster.moveSpeed *= value;
                break;
            case SpellUpgradeParameter.Cooldown:
                spellCaster.cooldown *= value;
                break;
            case SpellUpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= value;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void UpgradeBombSpell(BombSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, float value)
    {
        switch (spellUpgradeParameter)
        {
            case SpellUpgradeParameter.Damage:
                spellCaster.innerDamage = (int)(spellCaster.innerDamage * value);
                spellCaster.outerDamage = (int)(spellCaster.outerDamage * value);
                break;
            case SpellUpgradeParameter.Range:
                spellCaster.gravity *= value;
                spellCaster.initialVerticalVelocity *= value;
                spellCaster.initialForwardVelocity *= value;
                spellCaster.crosshairAimingClip *= value; // TODO test that modifying crosshairAimingClip makes sense
                break;
            case SpellUpgradeParameter.Cooldown:
                spellCaster.cooldown *= value;
                break;
            case SpellUpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= value;
                break;
            case SpellUpgradeParameter.Radius:
                spellCaster.aoeRadius *= value;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void UpgradeBubbleSpell(BubbleSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, float value)
    {
        switch (spellUpgradeParameter)
        {
            case SpellUpgradeParameter.Range:
                spellCaster.repelRadius *= value;
                break;
            case SpellUpgradeParameter.Cooldown:
                spellCaster.cooldown *= value;
                break;
            case SpellUpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= value;
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void UpgradeSniperSpell(SniperSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter, float value)
    {
        switch (spellUpgradeParameter)
        {
            case SpellUpgradeParameter.Damage:
                spellCaster.damage = (int)(spellCaster.damage * value);
                break;
            case SpellUpgradeParameter.Range:
                spellCaster.range *= value;
                break;
            case SpellUpgradeParameter.Cooldown:
                spellCaster.cooldown *= value;
                break;
            case SpellUpgradeParameter.Radius:
                spellCaster.spellScale *= value;
                break;
            case SpellUpgradeParameter.Speed:
                spellCaster.initialSpeed *= value;
                break;
            case SpellUpgradeParameter.MaxEnemiesHit:
                spellCaster.maxEnemiesCanHit += (int)value;
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
