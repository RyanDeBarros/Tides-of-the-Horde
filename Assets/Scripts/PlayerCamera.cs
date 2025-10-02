using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float panSpeed = 2f; // sensitivity

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        Assert.IsNotNull(cam);

        // Lock the cursor so it never hits screen edges
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * panSpeed;

        transform.Rotate(Vector3.up, mouseX);
    }

    public Camera GetCamera()
    {
        return cam;
    }

    public Vector3 GetForwardVector()
    {
        return cam.transform.forward;
    }
}
