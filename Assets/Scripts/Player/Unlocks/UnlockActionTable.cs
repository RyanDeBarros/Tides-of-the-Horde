using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
        return new UnlockActionRankedData() { int1 = int1[index], float1 = float1[index] };
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

    public void Load(List<UnlockActionData> listData)
    {
        listData.ForEach(d => data[d.id] = d);

        actions["f-MeleeSpell-Unlock"] = () => UnlockSpell(SpellType.Melee);
        actions["f-BombSpell-Unlock"] = () => UnlockSpell(SpellType.Bomb);
        actions["f-BubbleSpell-Unlock"] = () => UnlockSpell(SpellType.Bubble);
        actions["f-SniperSpell-Unlock"] = () => UnlockSpell(SpellType.Sniper);

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
        // TODO
    }

    private void UpgradeSpell(SpellType spell, SpellUpgradeParameter param, UnlockActionRankedData data)
    {
        // TODO
    }
}
