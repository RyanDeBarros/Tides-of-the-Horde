using UnityEngine;

public class DragonAOEAttack : MonoBehaviour
{
    [SerializeField] private float aoeRadius = 5f;
    [SerializeField] private float aoeFillSpeed = 10f;

    private bool exploding = false;
    private float explosionRadius = 0f;

    private void Update()
    {
        if (!exploding) return;

        explosionRadius += Time.deltaTime * aoeFillSpeed;
        if (explosionRadius >= aoeRadius)
            exploding = false;
    }

    public void Explode()
    {
        Debug.Log("Explode!");
        exploding = true;
        explosionRadius = 0f;
    }

    public bool IsExploding()
    {
        return exploding;
    }
}
