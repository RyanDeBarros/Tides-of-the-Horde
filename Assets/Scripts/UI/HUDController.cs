using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;
using System;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Header("Player References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerCurrency playerCurrency;

    [Serializable]
    private class SpellSelectTexture
    {
        public SpellType spellType;
        public Texture texture;
    }

    [Header("Spells")]
    [SerializeField] private List<SpellSelectController> spellSelectControllers;
    [SerializeField] private List<SpellSelectTexture> spells;

    private void Awake()
    {
        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(playerCurrency);
        Assert.IsTrue(spellSelectControllers.Count == spells.Count && spells.Count == Enum.GetValues(typeof(SpellType)).Length);
    }

    void Start()
    {
        // Subscribe to player status events
        playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
        UpdateHealthHUD(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
        playerCurrency.onCurrencyChanged.AddListener(UpdateCrystalsHUD);
        UpdateCrystalsHUD(playerCurrency.GetCurrency());
    }

    public void UpdateHealthHUD(int currentHP, int maxHP)
    {
        healthText.SetText($"{currentHP}/{maxHP}");
    }

    public void UpdateCrystalsHUD(int currentEXP)
    {
        crystalsText.SetText($"{currentEXP}");
    }

    public void SetSpellCooldowns(Dictionary<SpellType, float> cooldowns)
    {
        spellSelectControllers.ForEach(controller => {
            if (controller.IsUnlocked())
                controller.SetCooldown(cooldowns[controller.GetSpellType()]);
        });
    }

    public void UnlockSpell(SpellType spellType)
    {
        for (int i = 0; i < spellSelectControllers.Count; ++i)
        {
            var controller = spellSelectControllers[i];
            if (!controller.IsUnlocked())
            {
                controller.ShowUnlocked(i + 1, spellType, spells.First(spell => spell.spellType == spellType).texture);
                return;
            }
        }
    }

    public void LockSpell(SpellType spellType)
    {
        spellSelectControllers.Where(spell => spell.GetSpellType() == spellType).ToList().ForEach(spell => spell.ShowLocked());
    }

    public void UpdateSpellSelection(SpellType selectedSpell)
    {
        spellSelectControllers.ForEach(spell => {
            if (spell.GetSpellType() == selectedSpell)
                spell.ShowSelected();
            else
                spell.ShowDeselected();
        });
    }

    public SpellType GetMappedSpell(int keyNumber)
    {
        return spellSelectControllers.ToDictionary(controller => controller.GetNumberKey())[keyNumber].GetSpellType();
    }

    public int NumberOfUnlockedSpells()
    {
        return spellSelectControllers.Where(controller => controller.IsUnlocked()).Count();
    }
}
