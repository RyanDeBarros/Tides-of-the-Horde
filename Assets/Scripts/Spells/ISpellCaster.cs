using UnityEngine;

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
