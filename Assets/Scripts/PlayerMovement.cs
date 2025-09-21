using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.VisualScripting.Round<TInput, TOutput>;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject body;
    [SerializeField] private float walkingSpeed = 10f;
    [SerializeField] private float runningSpeed = 15f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float inputDeadzone = 0.15f; // ignore tiny axis values

    private CharacterController characterController;
    private PlayerCamera cam;
    private PlayerAnimatorController animator;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cam = GetComponent<PlayerCamera>();
        animator = GetComponentInChildren<PlayerAnimatorController>();
    }

    void Start()
    {
        Assert.IsNotNull(body);
        Assert.IsNotNull(characterController);
        Assert.IsNotNull(cam);
        Assert.IsNotNull(animator);
    }

    void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        if (Mathf.Abs(horizontal) > inputDeadzone || Mathf.Abs(vertical) > inputDeadzone)
        {
            bool running = Input.GetKey(KeyCode.LeftShift);
            MovePlayer(horizontal, vertical, running);
            animator.SetWalking(true);
            animator.SetRunning(running);
        }
        else
        {
            animator.SetWalking(false);
            animator.SetRunning(false);
        }
    }

    private void MovePlayer(float x, float z, bool running)
    {
        Vector3 forward = cam.GetForwardVector();
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 moveDir = (forward * z + right * x);
        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir = moveDir.normalized;

            float moveSpeed = running ? runningSpeed : walkingSpeed;
            characterController.Move(moveSpeed * Time.deltaTime * moveDir);

            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            body.transform.rotation = Quaternion.Slerp(
                body.transform.rotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }
}
