using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

// TODO separate into PlayerMovement and PlayerCamera scripts.
public class Player : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float panSpeed = 0.5f;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private float panX = 0f;
    private Vector3 velocity;
    private bool isGrounded;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
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
        UpdateGravity();
        UpdateMovement();
    }

    private void UpdateCamera()
    {
        float pan = Input.mousePosition.x - panX;
        panX = Input.mousePosition.x;
        pan *= panSpeed;
        cam.transform.RotateAround(transform.position, Vector3.up, pan);
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

    void MovePlayer(float x, float z)
    {
        Vector3 cameraDirection = cam.transform.forward;
        float cameraAngle = Mathf.Atan2(cameraDirection.z, cameraDirection.x);
        float inputAngle = Mathf.Atan2(z, x);
        float moveAngle = inputAngle - Mathf.PI / 2 + cameraAngle;
        Vector3 moveVector = new Vector3(Mathf.Cos(moveAngle), 0f, Mathf.Sin(moveAngle)) * moveSpeed;

        characterController.Move((velocity + moveVector) * Time.deltaTime);
        body.transform.forward = moveVector.normalized;
    }
}
