using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float panSpeed = 0.5f;

    private float panX = 0f;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        Assert.IsNotNull(cam);
        panX = Input.mousePosition.x;
    }
    
    void Update()
    {
        float pan = Input.mousePosition.x - panX;
        panX = Input.mousePosition.x;
        pan *= panSpeed;
        cam.transform.RotateAround(transform.position, Vector3.up, pan);
    }

    public Vector3 GetForwardVector()
    {
        return cam.transform.forward;
    }
}
