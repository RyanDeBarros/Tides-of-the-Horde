using UnityEngine;
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
    
    [Header("Demo Values")]
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int maxExp = 1000;
    public int currentExp = 0;

    void Start()
    {
        // Initialize the HUD with demo values
        UpdateHealthHUD(currentHealth, maxHealth);
        UpdateExpHUD(currentExp, maxExp);
    }
    

    public void UpdateHealthHUD(int currentHP, int maxHP)
    {
        // Update slider values
        healthBarSlider.maxValue = maxHP;
        healthBarSlider.value = currentHP;
        
        // Update text display
        if (healthText != null)
            healthText.text = $"{currentHP}/{maxHP}";
    }
    
    public void UpdateExpHUD(int currentEXP, int maxEXP)
    {
        // Update slider values
        expBarSlider.maxValue = maxEXP;
        expBarSlider.value = currentEXP;
        
        // Update text display
        if (expText != null)
            expText.text = $"{currentEXP}/{maxEXP}";
    }
}