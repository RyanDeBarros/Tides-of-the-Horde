using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MeleeSpell : MonoBehaviour
{
    public Vector3 blastPosition;
    public float lifetime = 0.3f;
    public int damage = 35;
    public float bounceBackStrength = 10f;

    public MeleeSpell_Shockwave shockwave;
    public MeleeSpell_Blast blast;
    private float timeElapsed = 0f;

    private readonly HashSet<GameObject> hitEnemies = new();

    void Awake()
    {
        shockwave = GetComponentInChildren<MeleeSpell_Shockwave>();
        blast = GetComponentInChildren<MeleeSpell_Blast>();
        Assert.IsNotNull(shockwave);
        Assert.IsNotNull(blast);
    }

    private void Start()
    {
        blast.transform.position = blastPosition;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > lifetime) Destroy(gameObject);
    }

    public void Hit(Collider target)
    {
        if (!hitEnemies.Contains(target.gameObject))
        {
            hitEnemies.Add(target.gameObject);
            Vector3 direction = target.transform.position - transform.position;
            TryBouncingBack(target, new Vector3(direction.x, 0f, direction.z).normalized);
            TryDamaging(target);
        }
    }

    private void TryBouncingBack(Collider target, Vector3 direction)
    {
        if (target.TryGetComponent<BounceBack>(out var bounceBack))
        {
            bounceBack.Bounce(direction, bounceBackStrength);
        }
    }

    private void TryDamaging(Collider target)
    {
        if (target.TryGetComponent<Health>(out var health))
        {
            health.TakeDamage(damage);
        }
    }
}
