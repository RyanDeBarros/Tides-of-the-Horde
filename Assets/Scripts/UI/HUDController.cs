using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI crystalsText;

    [Header("Player References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private SpellManager spellManager;

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
    private bool enableCameraOnResume = true;

    private void Awake()
    {
        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(playerCurrency);
        Assert.IsNotNull(playerCamera);
        Assert.IsNotNull(spellManager);
        Assert.IsTrue(spellSelectControllers.Count == spells.Count && spells.Count == Enum.GetValues(typeof(SpellType)).Length);
    }

    void Start()
    {
        // Subscribe to player status events
        playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
        UpdateHealthHUD(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
        playerCurrency.onCurrencyChanged.AddListener(UpdateCrystalsHUD);
        UpdateCrystalsHUD(playerCurrency.GetCurrency());
        playerHealth.onDeath.AddListener(ShowDeathScreen);

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

    public SpellType GetMappedSpell(int keyNumber)
    {
        return spellSelectControllers.ToDictionary(controller => controller.GetNumberKey())[keyNumber].GetSpellType();
    }

    public int NumberOfUnlockedSpells()
    {
        return spellSelectControllers.Where(controller => controller.IsUnlocked()).Count();
    }

    public void ShowDeathScreen()
    {
        Time.timeScale = 0f;
        deathScreenPanel.SetActive(true);
        playerCamera.DisableCamera();
    }

    public void Respawn()
    {
        deathScreenPanel.SetActive(false);
        Time.timeScale = 1f;
        playerCamera.EnableCamera();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        enableCameraOnResume = playerCamera.IsCameraEnabled();
        playerCamera.DisableCamera();
        spellManager.enabled = false;
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        if (enableCameraOnResume)
        {
            Time.timeScale = 1f;
            spellManager.enabled = true;
            playerCamera.EnableCamera();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
