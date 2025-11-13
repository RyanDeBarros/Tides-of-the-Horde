using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using System;
using UnityEngine.Rendering;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [Space]
    public float thresholdPercent = 10f; // Adjustable percentage threshold

    [Header("Events")]
    public UnityEvent<int, int> onHealthChanged; // (current, max)
    public UnityEvent onDeath;
    public UnityEvent onHealthThresholdReached; // New event for 10% intervals

    private bool invulnerable = false;
    private bool dead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        HealthChanged();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if (invulnerable) return;

        int previousHealth = currentHealth;
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        HealthChanged();

        // Check if we crossed a 10% threshold
        CheckHealthThreshold(previousHealth);

        if (currentHealth <= 0) Die();
    }

    private void CheckHealthThreshold(int previousHealth)
    {
        float thresholdInterval = maxHealth * (thresholdPercent / 100f);
        int currentThreshold = (int)(currentHealth / thresholdInterval);
        int previousThreshold = (int)(previousHealth / thresholdInterval);

        // If we crossed into a new threshold bracket
        if (currentThreshold < previousThreshold && currentHealth > 0
                && (previousHealth != maxHealth || currentThreshold != previousThreshold - 1))
            onHealthThresholdReached?.Invoke();
    }

    public void Heal(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        HealthChanged();
    }

    public void HealPercent(float fraction)
    {
        Heal((int)(fraction * maxHealth));
    }

    public void LowerToPercent(float fraction)
    {
        currentHealth = Math.Min(currentHealth, (int)(fraction * maxHealth));
        HealthChanged();
    }

    public void IncreaseMaxHealth(int amount)
    {
        Assert.IsTrue(amount >= 0);
        maxHealth += amount;
        HealthChanged();
    }

    public void SetInvulnerable(bool isInvulnerable)
    {
        invulnerable = isInvulnerable;
    }

    public bool IsInvulnerable()
    {
        return invulnerable;
    }

    public bool IsAlive()
    {
        return currentHealth > 0;
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            onDeath.Invoke();
        }
    }

    private void HealthChanged()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
