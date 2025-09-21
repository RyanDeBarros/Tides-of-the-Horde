using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float inputDeadzone = 0.15f; // ignore tiny axis values

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
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > inputDeadzone || Mathf.Abs(vertical) > inputDeadzone)
            MovePlayer(horizontal, vertical);
    }

    private void MovePlayer(float x, float z)
    {
        Vector3 forward = cam.GetForwardVector();
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 moveDir = (forward * z + right * x);
        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir = moveDir.normalized;

            characterController.Move(moveDir * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}
