using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("Health Bar References")]
    public GameObject healthBarContainer;
    public Slider healthSlider;

    [Header("Settings")]
    public float showDuration = 3f;
    public Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);

    private Health enemyHealth;
    private float hideTimer = 0f;
    private bool isVisible = false;

    void Start()
    {
        enemyHealth = GetComponent<Health>();
        healthBarContainer.SetActive(false);
        
        if (enemyHealth != null)
        {
            enemyHealth.onHealthChanged.AddListener(OnHealthChanged);
        }
    }

    void Update()
    {
        if (isVisible)
        {
            healthBarContainer.transform.position = transform.position + healthBarOffset;
            healthBarContainer.transform.rotation = Camera.main.transform.rotation;
            
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0f)
            {
                HideHealthBar();
            }
        }
    }

    public void OnHealthChanged(int currentHealth, int maxHealth)
    {
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

    void ShowHealthBar()
    {
        if (!isVisible)
        {
            healthBarContainer.SetActive(true);
            isVisible = true;
        }
    }

    void HideHealthBar()
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