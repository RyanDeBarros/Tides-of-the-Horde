using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Gravity : MonoBehaviour
{
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private float gravity = -10f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController characterController;

    private Vector3 velocity;
    private bool isGrounded = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        Assert.IsNotNull(groundCheck);
        Assert.IsNotNull(characterController);
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f; // small negative to stick to ground

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
