using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Health References")]
    public Slider healthBarSlider;
    public TextMeshProUGUI healthText;

    [Header("EXP References")]
    public Slider expBarSlider;
    public TextMeshProUGUI expText;

    [Header("Player References")]
    public Health playerHealth; // Assign in inspector

    [Header("Spells")]
    public List<SpellSelectController> spells;

    [Header("Demo EXP Values")]
    public int maxExp = 1000;
    public int currentExp = 0;

    void Start()
    {
        // Subscribe to player health events
        if (playerHealth != null)
        {
            playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
            UpdateHealthHUD(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
        }

        // Initialize EXP bar with demo values
        UpdateExpHUD(currentExp, maxExp);
    }

    public void UpdateHealthHUD(int currentHP, int maxHP)
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.maxValue = maxHP;
            healthBarSlider.value = currentHP;
        }

        if (healthText != null)
            healthText.text = $"{currentHP}/{maxHP}";
    }

    public void UpdateExpHUD(int currentEXP, int maxEXP)
    {
        if (expBarSlider != null)
        {
            expBarSlider.maxValue = maxEXP;
            expBarSlider.value = currentEXP;
        }

        if (expText != null)
            expText.text = $"{currentEXP}/{maxEXP}";
    }
}
