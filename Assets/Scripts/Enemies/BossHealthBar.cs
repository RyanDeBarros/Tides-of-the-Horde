using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Slider damageDelaySlider; 
    public TextMeshProUGUI bossNameText;
    public GameObject healthBarContainer;

    [Header("Animation Settings")]
    public float showAnimationDuration = 1.5f;
    public float hideAnimationDuration = 1f;

    private Health bossHealth;
    private CanvasGroup canvasGroup;
    private bool isShowing = false;
    private float showTimer = 0f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        healthBarContainer.SetActive(false);
        canvasGroup.alpha = 0f;
        InvokeRepeating("CheckForBoss", 0f, 1f);
    }

    void Update()
    {
        if (bossHealth == null) return;

        UpdateHealthBar();
        HandleShowHideAnimation();
    }

    public void InitializeBoss(Health bossHealthComponent, BossHealthBarTarget bossTarget)
    {
        bossHealth = bossHealthComponent;
        
        string bossName = bossTarget.displayName;
        bossNameText.text = bossName;

        bossHealth.onHealthChanged.AddListener(OnBossHealthChanged);
        bossHealth.onDeath.AddListener(OnBossDeath);

        healthSlider.maxValue = bossHealth.maxHealth;
        healthSlider.value = bossHealth.GetCurrentHealth();
        damageDelaySlider.maxValue = bossHealth.maxHealth;
        damageDelaySlider.value = bossHealth.GetCurrentHealth();

        ShowHealthBar();
    }

    void OnBossHealthChanged(int currentHealth, int maxHealth)
    {
        healthSlider.value = currentHealth;
    }

    void OnBossDeath()
    {
        HideHealthBar();
    }

    void UpdateHealthBar()
    {
        if (damageDelaySlider.value > healthSlider.value)
        {
            damageDelaySlider.value -= 30f * Time.deltaTime; 
        }
        else
        {
            damageDelaySlider.value = healthSlider.value;
        }
    }

    void ShowHealthBar()
    {
        isShowing = true;
        showTimer = 0f;
        healthBarContainer.SetActive(true);
    }

    void HideHealthBar()
    {
        isShowing = false;
        showTimer = 0f;
    }

    void HandleShowHideAnimation()
    {
        if (isShowing && canvasGroup.alpha < 1f)
        {
            showTimer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(showTimer / showAnimationDuration);
        }
        else if (!isShowing && canvasGroup.alpha > 0f)
        {
            showTimer += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(showTimer / hideAnimationDuration);

            if (canvasGroup.alpha <= 0f)
            {
                healthBarContainer.SetActive(false);
            }
        }
    }

    void CheckForBoss()
    {
        if (healthBarContainer.activeInHierarchy) return;
        
        BossHealthBarTarget bossTarget = FindObjectOfType<BossHealthBarTarget>();
        if (bossTarget != null)
        {
            Health bossHealth = bossTarget.GetComponent<Health>();
            if (bossHealth != null)
            {
                InitializeBoss(bossHealth, bossTarget);
                CancelInvoke("CheckForBoss"); 
            }
        }
    }
}