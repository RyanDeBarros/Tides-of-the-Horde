using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private float moveSpeed = 10f;

    private CharacterController characterController;
    private PlayerCamera cam;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponent<PlayerCamera>();
    }

    void Start()
    {
        Assert.IsNotNull(body);
        Assert.IsNotNull(characterController);
        Assert.IsNotNull(cam);
    }

    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (horizontal != 0 || vertical != 0)
            MovePlayer(horizontal, vertical);
    }

    private void MovePlayer(float x, float z)
    {
        Vector3 cameraDirection = cam.GetForwardVector();
        float cameraAngle = Mathf.Atan2(cameraDirection.z, cameraDirection.x);
        float inputAngle = Mathf.Atan2(z, x);
        float moveAngle = inputAngle - Mathf.PI / 2 + cameraAngle;
        Vector3 moveVector = new Vector3(Mathf.Cos(moveAngle), 0f, Mathf.Sin(moveAngle)) * moveSpeed;

        characterController.Move(moveVector * Time.deltaTime);
        body.transform.forward = moveVector.normalized;
    }
}
