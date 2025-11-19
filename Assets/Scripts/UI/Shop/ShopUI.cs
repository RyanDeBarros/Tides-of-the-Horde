using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject uiRoot;
    [SerializeField] private List<UnlockSelect> unlockSelects;

    [Header("Shop Disabled Banner")]
    [SerializeField] private GameObject shopPanel;              
    [SerializeField] private TextMeshProUGUI shopDisabledText;

    [Header("Check Shop Popup")]
    [SerializeField] private TextMeshProUGUI checkShopPopup;
    [SerializeField] private Image checkShopPopupBKG;
    [SerializeField] private float checkShopPopupFadeTime = 0.3f;
    [SerializeField] private float checkShopPopupDuration = 1f;
    [SerializeField] private int checkShopPopupRepetitions = 3;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI discountText;

    private PlayerEnabler player;
    private PlayerUnlockTree playerUnlock;
    private PlayerCurrency playerCurrency;
    
    private bool open = false;
    private Coroutine popup;
    private bool shopEnabled = true;

    private void Awake()
    {
        Assert.IsNotNull(uiRoot);
        Assert.IsNotNull(checkShopPopup);
        Assert.IsNotNull(checkShopPopupBKG);
        Assert.IsNotNull(shopPanel);
        Assert.IsNotNull(shopDisabledText);

        player = GlobalFind.FindUniqueObjectByType<PlayerEnabler>(true);
        playerUnlock = GlobalFind.FindUniqueObjectByType<PlayerUnlockTree>(true);
        playerCurrency = GlobalFind.FindUniqueObjectByType<PlayerCurrency>(true);

        SetCheckShopPopupAlpha(0f);

        shopPanel.SetActive(true);
        shopDisabledText.gameObject.SetActive(false);
    }

    private void Start()
    {
        uiRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            
           if (open)
               Close();
           else
               Open();
            
        }
    }

    private void Open()
    {
        if (open || !player.CameraEnabled()) return;

        open = true;
        Time.timeScale = 0f;
        uiRoot.SetActive(true);
        player.DisablePlayer();
        if (popup != null)
            StopCoroutine(popup);
        SetCheckShopPopupAlpha(0f);
        RefreshMetaInfo();
        if (shopEnabled)
        {
            
            shopPanel.SetActive(true);
            shopDisabledText.gameObject.SetActive(false);
        }
        else
        {
            
            shopPanel.SetActive(false);
            shopDisabledText.gameObject.SetActive(true);
            //I think the text should mostly be the same, but still keep it here in case you need to write something else
            //shopDisabledText.text = "According to the current challenge,\n" +
            //                        "the shop is not available in this level.";
        }
    }

    private void Close()
    {
        if (!open) return;

        open = false;
        Time.timeScale = 1f;
        uiRoot.SetActive(false);
        player.EnablePlayer();
    }

    public void RefreshOptions()
    {
        if (!shopEnabled)
            return;

        List<PlayerUnlockNode> nodes = playerUnlock.GetRandomUnlocks(unlockSelects.Count, playerCurrency.GetCurrency());
        for (int i = 0; i < unlockSelects.Count; ++i)
        {
            if (i < nodes.Count)
            {
                unlockSelects[i].ShowAvailable();
                unlockSelects[i].Setup(nodes[i]);
            }
            else
                unlockSelects[i].ShowUnavailable();
        }

        popup = StartCoroutine(AnimateCheckShopPopup());
    }

    private IEnumerator AnimateCheckShopPopup()
    {
        for (int i = 0; i < checkShopPopupRepetitions; ++i)
            yield return AnimateCheckPopupInstance();
    }

    private IEnumerator AnimateCheckPopupInstance()
    {
        // Fade in
        float t = 0f;
        while (t < checkShopPopupFadeTime)
        {
            t += Time.deltaTime;
            SetCheckShopPopupAlpha(Mathf.Clamp01(t / checkShopPopupFadeTime));
            yield return null;
        }
        SetCheckShopPopupAlpha(1f);

        // Stay
        t = 0f;
        while (t < checkShopPopupDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // Fade out
        t = 0f;
        while (t < checkShopPopupFadeTime)
        {
            t += Time.deltaTime;
            SetCheckShopPopupAlpha(1f - Mathf.Clamp01(t / checkShopPopupFadeTime));
            yield return null;
        }
        SetCheckShopPopupAlpha(0f);
    }

    private void SetCheckShopPopupAlpha(float alpha)
    {
        Color c = checkShopPopup.color;
        checkShopPopup.color = new Color(c.r, c.g, c.b, alpha);
        c = checkShopPopupBKG.color;
        checkShopPopupBKG.color = new Color(c.r, c.g, c.b, alpha);
    }

    public void EnableShop()
    {
        shopEnabled = true;
    }

    public void DisableShop()
    {
        shopEnabled = false;
    }

    private void RefreshMetaInfo()
    {
        if (playerCurrency == null)
            return;

        if (multiplierText != null)
        {
            float m = playerCurrency.GetMultiplier();
            
            multiplierText.text = $"Currency Gain: x{m:0.##}";
        }

        if (discountText != null)
        {
            float d = playerCurrency.GetShopDiscount(); // 0~1

            discountText.text = "Shop Discount: " + (d * 100f).ToString("0.#") + "%";
        }
    }

}
