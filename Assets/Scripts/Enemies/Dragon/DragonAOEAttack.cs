using UnityEngine;
using UnityEngine.Assertions;

public class DragonAOEAttack : MonoBehaviour
{
    [SerializeField] private SphereCollider explosionCollider;
    [SerializeField] private LayerMask playerLayer;

    public float aoeRadius = 10f;
    public float aoeFillSpeed = 20f;
    public int damage = 5;
    public float postExplosionDelay = 1f;
    public float cooldown = 3f;

    private Health playerHealth;

    private enum State
    {
        Ready,
        Exploding,
        PostExplosionDelay,
        Cooldown
    }

    private State state = State.Ready;

    private float timeElapsed = 0f;
    private float explosionRadius = 0f;
    private bool playerWasHit = false;
    private Transform colliderParent;
    private Vector3 colliderLocalPosition;

    private void Awake()
    {
        Assert.IsNotNull(explosionCollider);
        colliderParent = explosionCollider.transform.parent;
        Assert.IsNotNull(colliderParent);
        colliderLocalPosition = explosionCollider.transform.localPosition;

        var go = GameObject.FindGameObjectWithTag("Player").transform;
        Assert.IsNotNull(go);
        playerHealth = go.GetComponent<Health>();
        Assert.IsNotNull(playerHealth);

        Health myHealth = GetComponent<Health>();
        Assert.IsNotNull(myHealth);
        myHealth.onDeath.AddListener(CancelExplosion);

        SetExplosionRadius(0f);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Exploding:
                SetExplosionRadius(explosionRadius + Time.deltaTime * aoeFillSpeed);
                CheckForCollisions();
                if (explosionRadius >= aoeRadius)
                    CancelExplosion();
                break;
            case State.PostExplosionDelay:
                timeElapsed += Time.deltaTime;
                if (timeElapsed > postExplosionDelay)
                {
                    state = State.Cooldown;
                    timeElapsed = 0f;
                }
                break;
            case State.Cooldown:
                timeElapsed += Time.deltaTime;
                if (timeElapsed > cooldown)
                {
                    state = State.Ready;
                    timeElapsed = 0f;
                }
                break;
        }
    }

    private void SetExplosionRadius(float radius)
    {
        explosionCollider.transform.localScale = new(radius, radius, radius);
        explosionRadius = radius;
    }

    private void CheckForCollisions()
    {
        if (!playerWasHit)
        {
            Collider[] results = new Collider[1];
            if (Physics.OverlapSphereNonAlloc(explosionCollider.transform.position, explosionRadius, results, playerLayer) > 0)
            {
                playerWasHit = true;
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void Explode()
    {
        if (state == State.Ready)
        {
            state = State.Exploding;
            playerWasHit = false;
            SetExplosionRadius(0f);
            explosionCollider.transform.SetParent(null, true);
        }
    }

    public bool CanExplode()
    {
        return state == State.Ready;
    }

    public bool CanMove()
    {
        return state == State.Ready || state == State.Cooldown;
    }
    
    private void CancelExplosion()
    {
        SetExplosionRadius(0f);
        state = State.PostExplosionDelay;
        timeElapsed = 0f;
        explosionCollider.transform.SetParent(colliderParent, false);
        explosionCollider.transform.SetLocalPositionAndRotation(colliderLocalPosition, Quaternion.identity);
    }
}
