using UnityEngine;
using UnityEngine.Assertions;

public interface ISpellCaster
{
    public void Select();
    public void CastSpell(SpellManager manager);
}

public enum SpellType
{
    Melee,
    Bomb,
    Bubble,
    Sniper
}

public class UnlockableSpellCaster
{
    public ISpellCaster caster;
    public bool locked = true;

    public UnlockableSpellCaster(ISpellCaster caster)
    {
        this.caster = caster;
        Assert.IsNotNull(this.caster);
    }
}
