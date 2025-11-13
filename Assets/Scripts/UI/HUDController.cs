using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Header("Player References")]
    [SerializeField] private PlayerEnabler player;
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

    [Header("Death Screen")]
    public GameObject deathScreenPanel;
    public Button respawnButton;
    public Button mainMenuButton;

    [Header("Pause Menu")]
    public GameObject pauseMenuPanel;
    public Button resumeButton;
    public Button pauseMainMenuButton;

    private bool isPaused = false;
    private bool enablePlayerOnResume = true;

    private void Awake()
    {
        Assert.IsNotNull(player);
        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(playerCurrency);
        Assert.IsTrue(spellSelectControllers.Count == spells.Count && spells.Count == Enum.GetValues(typeof(SpellType)).Length);

        playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
        playerHealth.onDeath.AddListener(ShowDeathScreen);
        playerCurrency.onCurrencyChanged.AddListener(UpdateCrystalsHUD);
    }

    void Start()
    {
        // Subscribe to player status events
        UpdateHealthHUD(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
        UpdateCrystalsHUD(playerCurrency.GetCurrency());

        LoadSavedSettings();
    }

    void LoadSavedSettings()
    {
        if (PlayerPrefs.HasKey("GraphicsQuality"))
        {
            int savedQuality = PlayerPrefs.GetInt("GraphicsQuality");
            QualitySettings.SetQualityLevel(savedQuality);
        }

        if (PlayerPrefs.HasKey("MasterVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MasterVolume");
            AudioListener.volume = savedVolume;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
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

    public int GetSpellKey(SpellType spellType)
    {
        foreach (SpellSelectController controller in spellSelectControllers)
            if (controller.GetSpellType() == spellType)
                return controller.GetNumberKey();
        throw new KeyNotFoundException();
    }

    public SpellType GetMappedSpell(int keyNumber)
    {
        foreach (SpellSelectController controller in spellSelectControllers)
            if (controller.GetNumberKey() == keyNumber)
                return controller.GetSpellType();
        throw new IndexOutOfRangeException();
    }

    public int NumberOfUnlockedSpells()
    {
        return spellSelectControllers.Where(controller => controller.IsUnlocked()).Count();
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 0f;
        deathScreenPanel.SetActive(true);
        player.DisablePlayer();
    }

    public void Respawn()
    {
        Time.timeScale = 1f;
        SceneSwitcher.ReloadScene();
    }

    public void PauseGame()
    {
        StartCoroutine(PauseNextFrame());
    }

    private IEnumerator PauseNextFrame()
    {
        yield return new WaitForEndOfFrame();
        isPaused = true;
        Physics.SyncTransforms();
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        enablePlayerOnResume = player.CameraEnabled();
        player.DisablePlayer();
    }

    public void ResumeGame()
    {
        StartCoroutine(ResumeNextFrame());
    }

    private IEnumerator ResumeNextFrame()
    {
        yield return new WaitForEndOfFrame();
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        if (enablePlayerOnResume)
        {
            Physics.SyncTransforms();
            Time.timeScale = 1f;
            player.EnablePlayer();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneSwitcher.OpenMainMenu();
    }
}
