using UnityEngine;
using UnityEngine.Assertions;

public class PlayerEnabler : MonoBehaviour
{
    [SerializeField] private new PlayerCamera camera;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PlayerDash dash;
    [SerializeField] private SpellManager spellManager;

    private void Awake()
    {
        if (camera == null)
            camera = GetComponent<PlayerCamera>();
        Assert.IsNotNull(camera);

        if (movement == null)
            movement = GetComponent<PlayerMovement>();
        Assert.IsNotNull(movement);

        if (dash == null)
            dash = GetComponent<PlayerDash>();
        Assert.IsNotNull(dash);

        if (spellManager == null)
            spellManager = GetComponent<SpellManager>();
        Assert.IsNotNull(spellManager);
    }

    public void EnablePlayer()
    {
        camera.EnableCamera();
        movement.enabled = true;
        dash.enabled = true;
        spellManager.enabled = true;
    }

    public void DisablePlayer()
    {
        camera.DisableCamera();
        DisableInput();
    }

    public void DisableInput()
    {
        movement.enabled = false;
        dash.enabled = false;
        spellManager.enabled = false;
    }

    public bool CameraEnabled()
    {
        return camera.IsCameraEnabled();
    }
}
