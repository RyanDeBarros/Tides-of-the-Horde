using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BounceBack : MonoBehaviour
{
    [SerializeField] public float resistance = 1f;
    [SerializeField] public float duration = 0.1f;

    private CharacterController characterController;
    private Vector3 direction = Vector3.zero;
    private float elapsedTime = 0f;
    private float peakVelocity = 0f;
    private bool isBouncing = false;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(characterController);
    }

    private void Update()
    {
        if (!characterController.enabled)
            return;

        if (isBouncing)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            if (t >= 1f)
            {
                isBouncing = false;
                return;
            }

            float alpha = Mathf.Sin(Mathf.PI * Mathf.Pow(t, 0.2f));
            characterController.Move(alpha * peakVelocity * Time.deltaTime * direction);
        }
    }

    public void Bounce(Vector3 direction, float strength)
    {
        if (TryGetComponent(out Health health) && health.IsInvulnerable())
            return;

        this.direction = direction.normalized;
        peakVelocity = strength / resistance;
        elapsedTime = 0f;
        isBouncing = true;
    }
}
