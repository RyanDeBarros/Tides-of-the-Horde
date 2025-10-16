using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum UnlockAction
{
    Unlock,
    UpgradeSpell,
    // TODO UpgradeHealth
}

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

public class UnlockActionTable
{
    private readonly SpellManager spellManager;

    public UnlockActionTable()
    {
        spellManager = UnityEngine.Object.FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();
    }
    public Action<float> GetAction(string actionString, List<string> parameters)
    {
        UnlockAction action = Enum.Parse<UnlockAction>(actionString);
        return action switch
        {
            UnlockAction.Unlock => GetSpellUnlockAction(parameters),
            UnlockAction.UpgradeSpell => GetSpellUpgradeAction(parameters),
            _ => throw new NotImplementedException()
        };
    }

    private Action<float> GetSpellUnlockAction(List<string> parameters)
    {
        SpellType spellType = Enum.Parse<SpellType>(parameters[0]);
        return _ => spellManager.UnlockSpell(spellType);
    }

    private Action<float> GetSpellUpgradeAction(List<string> parameters)
    {
        SpellType spellType = Enum.Parse<SpellType>(parameters[0]);
        SpellUpgradeParameter upgradeParameter = Enum.Parse<SpellUpgradeParameter>(parameters[1]);
        return GetSpellUpgradeParameterAction(spellType, upgradeParameter);
    }

    private Action<float> GetSpellUpgradeParameterAction(SpellType spell, SpellUpgradeParameter param)
    {
        ISpellCaster spellCaster = spellManager.GetSpellCaster(spell);
        return spell switch
        {
            SpellType.Melee => GetUpgradeMeleeSpellAction((MeleeSpellCaster)spellCaster, param),
            SpellType.Bomb => GetUpgradeBombSpellAction((BombSpellCaster)spellCaster, param),
            SpellType.Bubble => GetUpgradeBubbleSpellAction((BubbleSpellCaster)spellCaster, param),
            SpellType.Sniper => GetUpgradeSniperSpellAction((SniperSpellCaster)spellCaster, param),
            _ => throw new NotImplementedException()
        };
    }

    private Action<float> GetUpgradeMeleeSpellAction(MeleeSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter)
    {
        return spellUpgradeParameter switch
        {
            SpellUpgradeParameter.Damage => value =>
                spellCaster.damage = (int)(spellCaster.damage * value),
            SpellUpgradeParameter.Range => value =>
                spellCaster.moveSpeed *= value,
            SpellUpgradeParameter.Cooldown => value =>
                spellCaster.cooldown *= value,
            SpellUpgradeParameter.BounceBack => value =>
                spellCaster.bounceBackStrength *= value,
            _ => throw new NotImplementedException()
        };
    }

    private Action<float> GetUpgradeBombSpellAction(BombSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter)
    {
        return spellUpgradeParameter switch
        {
            SpellUpgradeParameter.Damage => value =>
            {
                spellCaster.innerDamage = (int)(spellCaster.innerDamage * value);
                spellCaster.outerDamage = (int)(spellCaster.outerDamage * value);
            },
            SpellUpgradeParameter.Range => value =>
            {
                spellCaster.gravity *= value;
                spellCaster.initialVerticalVelocity *= value;
                spellCaster.initialForwardVelocity *= value;
                spellCaster.crosshairAimingClip *= value; // TODO test that modifying crosshairAimingClip makes sense
            },
            SpellUpgradeParameter.Cooldown => value =>
                spellCaster.cooldown *= value,
            SpellUpgradeParameter.BounceBack => value =>
                spellCaster.bounceBackStrength *= value,
            SpellUpgradeParameter.Radius => value =>
                spellCaster.aoeRadius *= value,
            _ => throw new NotImplementedException()
        };
    }

    private Action<float> GetUpgradeBubbleSpellAction(BubbleSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter)
    {
        return spellUpgradeParameter switch
        {
            SpellUpgradeParameter.Range => value =>
                spellCaster.repelRadius *= value,
            SpellUpgradeParameter.Cooldown => value =>
                spellCaster.cooldown *= value,
            SpellUpgradeParameter.BounceBack => value =>
                spellCaster.bounceBackStrength *= value,
            _ => throw new NotImplementedException()
        };
    }

    private Action<float> GetUpgradeSniperSpellAction(SniperSpellCaster spellCaster, SpellUpgradeParameter spellUpgradeParameter)
    {
        return spellUpgradeParameter switch
        {
            SpellUpgradeParameter.Damage => value =>
                spellCaster.damage = (int)(spellCaster.damage * value),
            SpellUpgradeParameter.Range => value =>
                spellCaster.range *= value,
            SpellUpgradeParameter.Cooldown => value =>
                spellCaster.cooldown *= value,
            SpellUpgradeParameter.Radius => value =>
                spellCaster.spellScale *= value,
            SpellUpgradeParameter.Speed => value =>
                spellCaster.initialSpeed *= value,
            SpellUpgradeParameter.MaxEnemiesHit => value =>
                spellCaster.maxEnemiesCanHit += (int)value,
            _ => throw new NotImplementedException()
        };
    }
}
