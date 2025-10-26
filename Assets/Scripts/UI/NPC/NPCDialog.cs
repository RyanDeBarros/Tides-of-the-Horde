using System;
using UnityEngine;
using UnityEngine.Assertions;

public class NPCDialog : MonoBehaviour
{
    public Action onClose;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerMovement playerMovement;

    private bool open = true;

    private void Awake()
    {
        camera = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None).GetUniqueElement();
        spellManager = FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();
        playerMovement = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None).GetUniqueElement();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (open && Input.GetKey(KeyCode.Escape))
            Close();
    }

    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;
        playerMovement.enabled = false;
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
        playerMovement.enabled = true;
        onClose.Invoke();
    }
}
