using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI crystalsText;
    [SerializeField] private TextMeshProUGUI healthAnim;
    [SerializeField] private TextMeshProUGUI crystalsAnim;
    [SerializeField] private float statsAnimationDuration = 0.2f;
    
    [SerializeField] private RawImage vignette;
    [SerializeField] private float lowHealthThreshold = 0.2f;
    
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gameOverAudioClip;

    [Header("Player References")]
    [SerializeField] private PlayerEnabler player;
    [SerializeField] private Health playerHealth;
    [SerializeField] private PlayerCurrency playerCurrency;
    [SerializeField] private PlayerAnimatorController playerAnimator;

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

    private class StatsAnimation
    {
        public TextMeshProUGUI text;
        public float lifetime = 0.2f;
        private float time = 0f;
        private int delta = 0;

        public void Start()
        {
            text.SetText("");
            time = lifetime;
        }

        public void Update()
        {
            if (time < lifetime)
            {
                time += Time.deltaTime;
                text.alpha = Mathf.Clamp01(1f - time / lifetime);
            }
            else
            {
                delta = 0;
                text.alpha = 0f;
            }
        }

        public void Animate(int d)
        {
            delta += d;
            if (delta != 0f)
            {
                text.SetText($"{(delta > 0 ? "+" : "")}{delta}");
                text.alpha = 1f;
                time = 0f;
            }
            else
                time = lifetime;
        }
    }

    private readonly StatsAnimation healthAnimation = new();
    private readonly StatsAnimation crystalsAnimation = new();

    private void Awake()
    {
        Assert.IsNotNull(healthText);
        Assert.IsNotNull(crystalsText);
        Assert.IsNotNull(healthAnim);
        Assert.IsNotNull(crystalsAnim);
        Assert.IsNotNull(vignette);
        Assert.IsNotNull(audioSource);

        Assert.IsNotNull(player);
        Assert.IsNotNull(playerHealth);
        Assert.IsNotNull(playerCurrency);
        Assert.IsNotNull(playerAnimator);
        Assert.IsTrue(spellSelectControllers.Count == spells.Count && spells.Count == Enum.GetValues(typeof(SpellType)).Length);

        healthText.SetText("");
        crystalsText.SetText("");

        healthAnimation.text = healthAnim;
        healthAnimation.lifetime = statsAnimationDuration;
        healthAnimation.Start();

        crystalsAnimation.text = crystalsAnim;
        crystalsAnimation.lifetime = statsAnimationDuration;
        crystalsAnimation.Start();

        playerHealth.onHealthChanged.AddListener(UpdateHealthHUD);
        playerHealth.onDeath.AddListener(() => StartCoroutine(DeathTransition()));
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

        if (!isPaused)
        {
            healthAnimation.Update();
            crystalsAnimation.Update();
        }
    }

    public void UpdateHealthHUD(int currentHP, int maxHP)
    {
        if (healthText.text != "")
            healthAnimation.Animate(currentHP - int.Parse(healthText.text.Split('/')[0]));
        
        healthText.SetText($"{currentHP}/{maxHP}");
        Vector2 sz = healthText.rectTransform.sizeDelta;
        healthText.rectTransform.sizeDelta = new(healthText.preferredWidth, sz.y);

        UpdateVignette((float)currentHP / maxHP);
    }

    private void UpdateVignette(float normalizedHP)
    {
        Color color = vignette.color;
        color.a = Mathf.Clamp01(1f - normalizedHP / lowHealthThreshold);
        vignette.color = color;
    }

    public void UpdateCrystalsHUD(int currentEXP)
    {
        if (crystalsText.text != "")
            crystalsAnimation.Animate(currentEXP - int.Parse(crystalsText.text));

        crystalsText.SetText($"{currentEXP}");
        Vector2 sz = crystalsText.rectTransform.sizeDelta;
        crystalsText.rectTransform.sizeDelta = new(crystalsText.preferredWidth, sz.y);
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

    private IEnumerator DeathTransition()
    {
        player.DisableInput();
        yield return playerAnimator.PlayDeathAnimation();

        if (gameOverAudioClip != null)
            audioSource.PlayOneShot(gameOverAudioClip);
        // TODO play game over music

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
