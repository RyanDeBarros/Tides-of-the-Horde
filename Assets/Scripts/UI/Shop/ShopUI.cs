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

    private void Awake()
    {
        PlayerCamera[] cameras = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None);
        Assert.IsTrue(cameras.Length == 1);
        camera = cameras[0];

        SpellManager[] spellManagers = FindObjectsByType<SpellManager>(FindObjectsSortMode.None);
        Assert.IsTrue(spellManagers.Length == 1);
        spellManager = spellManagers[0];

        PlayerUnlockTree[] playerUnlocks = FindObjectsByType<PlayerUnlockTree>(FindObjectsSortMode.None);
        Assert.IsTrue(playerUnlocks.Length == 1);
        playerUnlock = playerUnlocks[0];
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
        List<PlayerUnlockNode> nodes = playerUnlock.GetRandomUnlocks(unlockSelects.Count);
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
