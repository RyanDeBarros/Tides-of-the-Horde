using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Health targetHealth;

    void Start()
    {
        if (targetHealth != null)
        {
            targetHealth.onHealthChanged.AddListener(UpdateHealthBar);
            UpdateHealthBar(targetHealth.currentHealth, targetHealth.maxHealth);
        }
    }

    void UpdateHealthBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
