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
        SpellUpgradeParameter upgradeParameter = Enum.Parse<SpellUpgradeParameter>(parameters[1]);
        return values => UpgradeSpell(spellType, upgradeParameter, values[0]);
    }

    private void UpgradeSpell(SpellType spell, SpellUpgradeParameter param, float value)
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
        };
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
        };
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
        };
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
        };
    }

    private Action<float[]> GetHealthUpgradeAction()
    {
        return values => {
            float maxHealthIncrease = values[0];
            float healPercent = values[1];
            playerHealth.IncreaseMaxHealth((int)maxHealthIncrease);
            playerHealth.HealPercent(healPercent);
        };
    }

    private Action<float[]> GetDashUpgradeAction(List<string> parameters)
    {
        // TODO
        return values => { };
    }
}
