using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Assertions;
using System;

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

    [SerializeField] private string dieStateName = "Die"; 
    [SerializeField] private float extraDelay = 0.15f;    
    [SerializeField] private Behaviour[] disableOnDeath;

    [Header("Sink Settings")]
    [SerializeField] private float sinkDuration = 1.5f;
    [SerializeField] private float sinkSpeed = 0.6f;

    Animator anim;
    bool dead;

    void Awake()
    {
        currentHealth = maxHealth;
        HealthChanged();
        anim = GetComponentInChildren<Animator>();
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

        HealthChanged();

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

    public void Die()
    {
        if (dead) return;
        dead = true;

        onDeath?.Invoke();

        
        foreach (var b in disableOnDeath) if (b) b.enabled = false;
        var agent = GetComponent<NavMeshAgent>(); if (agent) agent.enabled = false;
        var cc = GetComponent<CharacterController>(); if (cc) cc.enabled = false;
        var rb = GetComponent<Rigidbody>(); if (rb) rb.isKinematic = true;
        var col = GetComponent<Collider>(); if (col) col.enabled = false; 

        if (anim)
        {
            anim.ResetTrigger("Hit"); 
            anim.SetTrigger("Die");

            
            float len = 0f;
            var rc = anim.runtimeAnimatorController;
            if (rc != null)
                foreach (var clip in rc.animationClips)
                    if (clip.name == dieStateName) { len = clip.length; break; }
            if (len <= 0f) len = 2f;

            StartCoroutine(PlayDieThenSink(len + extraDelay));
        }
        else
        {
            
            StartCoroutine(SinkAndDestroy());
        }
    }

    System.Collections.IEnumerator PlayDieThenSink(float wait)
    {
        yield return new WaitForSeconds(wait);
        yield return SinkAndDestroy();
    }

    System.Collections.IEnumerator SinkAndDestroy()
    {
        float t = 0f;
        while (t < sinkDuration)
        {
            transform.Translate(Vector3.down * sinkSpeed * Time.deltaTime, Space.World);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private void HealthChanged()
    {
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
