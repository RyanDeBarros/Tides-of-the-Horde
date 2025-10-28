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

    [Header("Check Shop Popup")]
    [SerializeField] private TextMeshProUGUI checkShopPopup;
    [SerializeField] private Image checkShopPopupBKG;
    [SerializeField] private float checkShopPopupFadeTime = 0.3f;
    [SerializeField] private float checkShopPopupDuration = 1f;
    [SerializeField] private int checkShopPopupRepetitions = 3;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerUnlockTree playerUnlock;
    private PlayerCurrency playerCurrency;
    private bool open = true;
    private Coroutine popup;
    private bool shopEnabled = true;

    private void Awake()
    {
        Assert.IsNotNull(uiRoot);
        Assert.IsNotNull(checkShopPopup);
        Assert.IsNotNull(checkShopPopupBKG);

        camera = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None).GetUniqueElement();
        spellManager = FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();
        playerUnlock = FindObjectsByType<PlayerUnlockTree>(FindObjectsSortMode.None).GetUniqueElement();
        playerCurrency = FindObjectsByType<PlayerCurrency>(FindObjectsSortMode.None).GetUniqueElement();

        SetCheckShopPopupAlpha(0f);
    }

    private void Start()
    {
        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (shopEnabled)
            {
                if (open)
                    Close();
                else
                    Open();
            }
            // TODO else popup message saying shop is disabled?
        }
    }

    private void Open()
    {
        open = true;
        Time.timeScale = 0f;
        uiRoot.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;
        StopCoroutine(popup);
        SetCheckShopPopupAlpha(0f);
    }

    private void Close()
    {
        open = false;
        Time.timeScale = 1f;
        uiRoot.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
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
}
