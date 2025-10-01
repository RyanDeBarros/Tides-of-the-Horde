using UnityEngine;

public interface ISpellCaster
{
    public void CastSpell(Vector3 playerPosition, Vector3 staffPosition, Vector3 playerDirection, Vector3 cameraDirection);
}
