using UnityEngine;
using UnityEngine.Assertions;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlayerCamera cam;
    [SerializeField] private float inputDeadzone = 0.15f; // ignore tiny axis values

    public float dashDuration = 0.15f;
    public float dashSpeed = 30f;
    public float cooldown = 1f;

    private bool unlocked = false;
    private float timeElapsed = 0f;
    private bool dashing = false;
    private Vector3 dashDir = Vector3.zero;

    private void Awake()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
        Assert.IsNotNull(characterController);

        if (cam == null)
            cam = GetComponent<PlayerCamera>();
        Assert.IsNotNull(cam);
    }

    public void Unlock()
    {
        unlocked = true;
    }

    private void Update()
    {
        if (!unlocked)
            return;

        if (dashing)
        {
            timeElapsed -= Time.deltaTime;
            if (timeElapsed > 0f)
                characterController.Move(dashSpeed * Time.deltaTime * dashDir);
            else
            {
                dashing = false;
                timeElapsed = cooldown;
            }
        }
        else
        {
            if (timeElapsed > 0f)
                timeElapsed -= Time.deltaTime;
        
            if (Input.GetKeyDown(KeyCode.Space))
                TryDashing();
        }
    }

    public void TryDashing()
    {
        if (dashing || timeElapsed > 0f)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D)) horizontal += 1f;

        if (Input.GetKey(KeyCode.W)) vertical += 1f;
        if (Input.GetKey(KeyCode.S)) vertical -= 1f;

        if (Mathf.Abs(horizontal) > inputDeadzone || Mathf.Abs(vertical) > inputDeadzone)
        {
            dashing = true;
            timeElapsed = dashDuration;

            Vector3 forward = cam.GetForwardVector();
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, forward);
            dashDir = (forward * vertical + right * horizontal).normalized;
        }
    }

    public float GetNormalizedCooldown()
    {
        if (dashing)
            return 1f;
        else if (timeElapsed > 0f)
            return Mathf.Clamp01(timeElapsed / cooldown);
        else
            return 0f;
    }
}
