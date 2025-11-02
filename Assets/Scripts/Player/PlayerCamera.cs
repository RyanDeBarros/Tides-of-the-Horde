using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float horizontalPanSpeed = 2f; // sensitivity
    [SerializeField] private float verticalPanSpeed = 2f; // sensitivity
    [SerializeField] private float verticalPanMin = -15f; // relative to initial pitch
    [SerializeField] private float verticalPanMax = 30f; // relative to initial pitch

    private float initialPitch = 0f;
    private float currentPitch = 0f;
    private bool cameraEnabled = true;

    private void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        Assert.IsNotNull(cam);

        initialPitch = cam.transform.eulerAngles.x;
        if (initialPitch > 180f) initialPitch -= 360f; // convert to [-180, 180] range
        currentPitch = initialPitch;

        EnableCamera();
    }

    void Update()
    {
        if (!cameraEnabled)
            return;

        float mouseX = Input.GetAxis("Mouse X") * horizontalPanSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalPanSpeed;

        transform.Rotate(Vector3.up, mouseX);

        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, initialPitch - verticalPanMax, initialPitch - verticalPanMin);

        // Compute the rotation needed to achieve currentPitch
        float deltaPitch = currentPitch - cam.transform.eulerAngles.x;
        if (deltaPitch > 180f) deltaPitch -= 360f;
        if (deltaPitch < -180f) deltaPitch += 360f;

        // Rotate camera around player position along the right axis
        cam.transform.RotateAround(transform.position, transform.right, deltaPitch);
    }

    public Vector3 GetForwardVector()
    {
        return cam.transform.forward;
    }

    public void EnableCamera()
    {
        cameraEnabled = true;
        // Lock the cursor so it never hits screen edges
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableCamera()
    {
        cameraEnabled = false;
        // Restore cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
