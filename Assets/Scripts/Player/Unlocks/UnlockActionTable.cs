using System;
using System.Collections.Generic;

[Serializable]
public class UnlockActionTableData
{

}

public class UnlockActionTable
{
    private Dictionary<string, Action> actions;

    public void Load(UnlockActionTableData data)
    {
        actions = new() {
            { "f-MeleeSpell-Unlock", () => UnlockSpell(SpellType.Melee) },
            { "f-BombSpell-Unlock", () => UnlockSpell(SpellType.Bomb) },
            { "f-BubbleSpell-Unlock", () => UnlockSpell(SpellType.Bubble) },
            { "f-SniperSpell-Unlock", () => UnlockSpell(SpellType.Sniper) },

            // TODO
            { "f-MeleeSpell-Upgrade-Damage-1", () => { } },
            { "f-BombSpell-Upgrade-Damage-1", () => { } },
            { "f-BombSpell-Upgrade-Range-1", () => { } },
            { "f-BombSpell-Upgrade-Range-2", () => { } },
        };
    }

    public Action GetAction(string name)
    {
        return actions[name];
    }

    private void UnlockSpell(SpellType spell)
    {
        // TODO
    }
}
