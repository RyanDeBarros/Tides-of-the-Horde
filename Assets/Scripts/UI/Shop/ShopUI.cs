using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ShopUI : MonoBehaviour
{
    private new PlayerCamera camera;

    private void Awake()
    {
        camera = FindFirstObjectByType<PlayerCamera>();
        Assert.IsNotNull(camera);
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
    }

    public void Close()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
        camera.EnableCamera();
    }
}
