using UnityEngine;
using UnityEngine.Assertions;

public class BossHealthBarTarget : MonoBehaviour
{
    public string displayName = "BOSS";

    [SerializeField] private Health health;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        Assert.IsNotNull(health);
    }

    private void Start()
    {
        BossHealthBar healthBar = FindFirstObjectByType<BossHealthBar>();
        if (healthBar != null)
        {
            healthBar.InitializeBoss(health, this);
        }
    }
}