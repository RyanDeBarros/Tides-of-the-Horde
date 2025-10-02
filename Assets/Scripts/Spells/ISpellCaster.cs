using UnityEngine;

public interface ISpellCaster
{
    public void Select();
    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Transform player);
}

public enum SpellType
{
    Melee,
    Bomb,
    Bubble,
    Sniper
}
