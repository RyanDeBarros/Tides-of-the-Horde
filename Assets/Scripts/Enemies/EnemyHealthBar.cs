using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar References")]
    public GameObject healthBarContainer;
    public Slider healthSlider;
    [SerializeField] private Health enemyHealth;

    [Header("Settings")]
    public float showDuration = 3f;

    private float hideTimer = 0f;
    private bool isVisible = false;

    void Start()
    {
        enemyHealth = GetComponent<Health>();
        Assert.IsNotNull(enemyHealth);
        
        enemyHealth.onHealthChanged.AddListener(OnHealthChanged);
        healthBarContainer.SetActive(false);
    }

    void Update()
    {
        if (isVisible)
        {
            healthBarContainer.transform.rotation = Camera.main.transform.rotation;
            
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                HideHealthBar();
            }
        }
    }

    private void OnDisable()
    {
        HideHealthBar();
    }

    public void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (!enabled) return;

        float healthPercent = (float)currentHealth / maxHealth;
        healthSlider.value = healthPercent;
        
        // only show if damage taken
        if (currentHealth < maxHealth && currentHealth > 0)
        {
            ShowHealthBar();
            hideTimer = showDuration; 
        }
        
        if (currentHealth <= 0)
        {
            HideHealthBar();
        }
    }

    private void ShowHealthBar()
    {
        if (!isVisible)
        {
            healthBarContainer.SetActive(true);
            isVisible = true;
        }
    }

    private void HideHealthBar()
    {
        if (isVisible)
        {
            healthBarContainer.SetActive(false);
            isVisible = false;
        }
    }

    public void ForceShowTemporarily()
    {
        ShowHealthBar();
        hideTimer = showDuration;
    }
}