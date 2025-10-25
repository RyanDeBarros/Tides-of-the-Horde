using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    [Space]
    public float thresholdPercent = 10f; // Adjustable percentage threshold

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged; // (current, max)
    public UnityEvent onDeath;
    public UnityEvent onHealthThresholdReached; // New event for 10% intervals

    void Awake()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        int previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);

        // Check if we crossed a 10% threshold
        CheckHealthThreshold(previousHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void CheckHealthThreshold(int previousHealth)
    {
        float thresholdInterval = maxHealth * (thresholdPercent / 100f);
        int currentThreshold = (int)(currentHealth / thresholdInterval);
        int previousThreshold = (int)(previousHealth / thresholdInterval);

        // If we crossed into a new threshold bracket
        if (currentThreshold < previousThreshold && currentHealth > 0)
        {
            onHealthThresholdReached?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void HealPercent(float fraction)
    {
        Heal((int)(fraction * maxHealth));
    }

    public void IncreaseMaxHealth(int amount)
    {
        Assert.IsTrue(amount >= 0);
        maxHealth += amount;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        onDeath?.Invoke();
        Destroy(gameObject);
    }
}