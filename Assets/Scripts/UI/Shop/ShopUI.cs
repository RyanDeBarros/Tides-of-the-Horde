using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopUI : MonoBehaviour
{
    private new PlayerCamera camera;
    private SpellManager spellManager;

    private void Awake()
    {
        PlayerCamera[] cameras = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None);
        Assert.IsTrue(cameras.Length == 1);
        camera = cameras[0];

        SpellManager[] spellManagers = FindObjectsByType<SpellManager>(FindObjectsSortMode.None);
        Assert.IsTrue(spellManagers.Length == 1);
        spellManager = spellManagers[0];
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
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
    }
}
