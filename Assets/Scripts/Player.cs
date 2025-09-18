using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject body;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float panSpeed = 0.5f;

    private float panX = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        Assert.IsNotNull(body);
        panX = Input.mousePosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCamera();
        // TODO apply gravity
        UpdateMovement();
    }

    void UpdateCamera()
    {
        float pan = Input.mousePosition.x - panX;
        panX = Input.mousePosition.x;
        pan *= panSpeed;
        cam.transform.RotateAround(transform.position, Vector3.up, pan);
    }

    void UpdateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
            MovePlayer(horizontal, vertical);
    }

    void MovePlayer(float x, float z)
    {
        Vector3 cameraDirection = cam.transform.forward;
        float cameraAngle = Mathf.Atan2(cameraDirection.z, cameraDirection.x);
        float inputAngle = Mathf.Atan2(z, x);
        float moveAngle = inputAngle - Mathf.PI / 2 + cameraAngle;
        Vector3 moveVector = new(Mathf.Cos(moveAngle), 0f, Mathf.Sin(moveAngle));
        moveVector *= Time.deltaTime * moveSpeed;
        
        transform.position += moveVector;
        body.transform.forward = moveVector.normalized;
    }
}
