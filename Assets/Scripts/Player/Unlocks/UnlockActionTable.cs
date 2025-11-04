using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public enum UnlockAction
{
    Unlock,
    UnlockDash,
    UpgradeSpell,
    UpgradeHealth,
    UpgradeDash
}

public enum UpgradeParameter
{
    Damage,
    Range,
    Cooldown,
    BounceBack,
    Radius,
    Speed,
    Duration,
    MaxEnemiesHit
}

public class UnlockActionTable
{
    private readonly SpellManager spellManager;
    private readonly Health playerHealth;
    private readonly PlayerDash playerDash;
    private readonly DashCooldownUI dashUI;

    public UnlockActionTable(GameObject player, DashCooldownUI dashUI)
    {
        spellManager = player.GetComponent<SpellManager>();
        playerHealth = player.GetComponent<Health>();
        playerDash = player.GetComponent<PlayerDash>();
        this.dashUI = dashUI;

        Assert.IsNotNull(spellManager);
        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(playerDash);
        Assert.IsNotNull(dashUI);
    }
    public Action<float[]> GetAction(string actionString, List<string> parameters)
    {
        UnlockAction action = Enum.Parse<UnlockAction>(actionString);
        return action switch
        {
            UnlockAction.Unlock => GetSpellUnlockAction(parameters),
            UnlockAction.UnlockDash => GetDashUnlockAction(),
            UnlockAction.UpgradeSpell => GetSpellUpgradeAction(parameters),
            UnlockAction.UpgradeHealth => GetHealthUpgradeAction(),
            UnlockAction.UpgradeDash => GetDashUpgradeAction(parameters),
            _ => throw new NotImplementedException()
        };
    }

    private Action<float[]> GetSpellUnlockAction(List<string> parameters)
    {
        SpellType spellType = Enum.Parse<SpellType>(parameters[0]);
        return _ => spellManager.UnlockSpell(spellType);
    }

    private Action<float[]> GetDashUnlockAction()
    {
        return _ => { playerDash.Unlock(); dashUI.Show(); };
    }

    private Action<float[]> GetSpellUpgradeAction(List<string> parameters)
    {
        SpellType spellType = Enum.Parse<SpellType>(parameters[0]);
        UpgradeParameter upgradeParameter = Enum.Parse<UpgradeParameter>(parameters[1]);
        return values => UpgradeSpell(spellType, upgradeParameter, values[0]);
    }

    private void UpgradeSpell(SpellType spell, UpgradeParameter param, float value)
    {
        ISpellCaster spellCaster = spellManager.GetSpellCaster(spell);
        switch (spell)
        {
            case SpellType.Melee:
                UpgradeMeleeSpell((MeleeSpellCaster)spellCaster, param, value);
                break;
            case SpellType.Bomb:
                UpgradeBombSpell((BombSpellCaster)spellCaster, param, value);
                break;
            case SpellType.Bubble:
                UpgradeBubbleSpell((BubbleSpellCaster)spellCaster, param, value);
                break;
            case SpellType.Sniper:
                UpgradeSniperSpell((SniperSpellCaster)spellCaster, param, value);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private void UpgradeMeleeSpell(MeleeSpellCaster spellCaster, UpgradeParameter upgradeParameter, float value)
    {
        switch (upgradeParameter)
        {
            case UpgradeParameter.Damage:
                spellCaster.damage = (int)(spellCaster.damage * (1f + value));
                break;
            case UpgradeParameter.Range:
                spellCaster.moveSpeed *= (1f + value);
                break;
            case UpgradeParameter.Cooldown:
                spellCaster.cooldown *= (1f - value);
                break;
            case UpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= (1f + value);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private void UpgradeBombSpell(BombSpellCaster spellCaster, UpgradeParameter upgradeParameter, float value)
    {
        switch (upgradeParameter)
        {
            case UpgradeParameter.Damage:
                value += 1f;
                spellCaster.innerDamage = (int)(spellCaster.innerDamage * value);
                spellCaster.outerDamage = (int)(spellCaster.outerDamage * value);
                break;
            case UpgradeParameter.Range:
                value += 1f;
                spellCaster.gravity *= value;
                spellCaster.initialVerticalVelocity *= value;
                spellCaster.initialForwardVelocity *= value;
                break;
            case UpgradeParameter.Cooldown:
                spellCaster.cooldown *= (1f - value);
                break;
            case UpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= (1f + value);
                break;
            case UpgradeParameter.Radius:
                spellCaster.aoeRadius *= (1f + value);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private void UpgradeBubbleSpell(BubbleSpellCaster spellCaster, UpgradeParameter upgradeParameter, float value)
    {
        switch (upgradeParameter)
        {
            case UpgradeParameter.Range:
                spellCaster.repelRadius *= (1f + value);
                break;
            case UpgradeParameter.Cooldown:
                spellCaster.cooldown *= (1f - value);
                break;
            case UpgradeParameter.BounceBack:
                spellCaster.bounceBackStrength *= (1f + value);
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private void UpgradeSniperSpell(SniperSpellCaster spellCaster, UpgradeParameter upgradeParameter, float value)
    {
        switch (upgradeParameter)
        {
            case UpgradeParameter.Damage:
                spellCaster.damage = (int)(spellCaster.damage * (1f + value));
                break;
            case UpgradeParameter.Range:
                spellCaster.range *= (1f + value);
                break;
            case UpgradeParameter.Cooldown:
                spellCaster.cooldown *= (1f - value);
                break;
            case UpgradeParameter.Radius:
                spellCaster.spellScale *= (1f + value);
                break;
            case UpgradeParameter.Speed:
                spellCaster.initialSpeed *= (1f + value);
                break;
            case UpgradeParameter.MaxEnemiesHit:
                spellCaster.maxEnemiesCanHit += (int)value;
                break;
            default:
                throw new NotImplementedException();
        };
    }

    private Action<float[]> GetHealthUpgradeAction()
    {
        return values => {
            playerHealth.IncreaseMaxHealth((int)values[0]);
            playerHealth.HealPercent(values[1]);
        };
    }

    private Action<float[]> GetDashUpgradeAction(List<string> parameters)
    {
        switch (Enum.Parse<UpgradeParameter>(parameters[0]))
        {
            case UpgradeParameter.Cooldown:
                return values => playerDash.cooldown *= (1f - values[0]);
            case UpgradeParameter.Speed:
                return values => playerDash.dashSpeed *= (1f + values[0]);
            case UpgradeParameter.Duration:
                return values => playerDash.dashDuration *= (1f + values[0]);
            default:
                throw new NotImplementedException();
        }
    }
}
