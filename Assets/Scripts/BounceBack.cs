using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BounceBack : MonoBehaviour
{
    [SerializeField] private float resistance = 1f;

    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(characterController);
    }

    public void Bounce(Vector3 direction, float strength)
    {
        strength /= resistance;
        characterController.Move(direction.normalized * strength);
    }
}
