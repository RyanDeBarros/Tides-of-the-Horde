using System.Collections.Generic;
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

    [Header("Spells")]
    [SerializeField] private List<SpellSelectController> spells;

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
    }

    void Start()
    {
        // Subscribe to player status events
        playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
        UpdateHealthHUD(playerHealth.GetCurrentHealth(), playerHealth.maxHealth);
        playerCurrency.onCurrencyChanged.AddListener(UpdateCrystalsHUD);
        UpdateCrystalsHUD(playerCurrency.GetCurrency());
        playerHealth.onDeath.AddListener(ShowDeathScreen);

        for (int i = 0; i < spells.Count; ++i)
            spells[i].SetKeyHint(i + 1);

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

    public List<SpellSelectController> GetSpells()
    {
        return spells;
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
