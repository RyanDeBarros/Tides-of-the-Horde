using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController characterController;
    private PlayerCamera cam;
    
    private Vector3 velocity;
    private bool isGrounded;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponent<PlayerCamera>();
    }

    void Start()
    {
        Assert.IsNotNull(body);
        Assert.IsNotNull(groundCheck);
    }

    void Update()
    {
        UpdateGravity();
        UpdateMovement();
    }

    private void UpdateGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small negative to stick to ground

        velocity.y += gravity * Time.deltaTime;
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

        characterController.Move((velocity + moveVector) * Time.deltaTime);
        body.transform.forward = moveVector.normalized;
    }
}
