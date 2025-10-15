using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private List<UnlockSelect> unlockSelects;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerUnlockTree playerUnlock;
    private PlayerCurrency playerCurrency;

    private void Awake()
    {
        camera = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None).GetUniqueElement();
        spellManager = FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();
        playerUnlock = FindObjectsByType<PlayerUnlockTree>(FindObjectsSortMode.None).GetUniqueElement();
        playerCurrency = FindObjectsByType<PlayerCurrency>(FindObjectsSortMode.None).GetUniqueElement();
    }

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        Time.timeScale = 0f;
        gameObject.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;

        RefreshOptions();
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
    }

    private void RefreshOptions()
    {
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
    }
}
