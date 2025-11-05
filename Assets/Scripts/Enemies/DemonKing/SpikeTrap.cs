using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

class SpikeTrap : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private GameObject spikesVFX;
    [SerializeField] private GameObject telegraphVFX;

    private bool firstEntered = true;
    public int initialDamage = 0;
    public float damageOverTime = 0f;
    private float damageDebt = 0f;
    public float slowingFactor = 1f;

    private PlayerMovement playerMovement = null;

    private enum State
    {
        Inactive,
        Telegraphing,
        Rising,
        Staying,
        Falling
    }

    private State state = State.Inactive;
    private float timeElapsed = 0f;
    public float telegraphDuration = 0f;
    public float risingDuration = 0f;
    public float stayingDuration = 0f;
    public float fallingDuration = 0f;

    private void Awake()
    {
        Assert.IsNotNull(collider);
        Assert.IsNotNull(spikesVFX);
        Assert.IsNotNull(telegraphVFX);
    }

    private void Start()
    {
        state = State.Inactive;
        collider.enabled = false;
        spikesVFX.SetActive(false);
        telegraphVFX.SetActive(false);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Telegraphing:
                timeElapsed += Time.deltaTime;
                if (timeElapsed < telegraphDuration)
                    SetTelegraphScale(timeElapsed / telegraphDuration);
                else
                {
                    SetTelegraphScale(1f);
                    spikesVFX.SetActive(true);
                    SetSpikesScale(0f);
                    timeElapsed = 0f;
                    state = State.Rising;
                    collider.enabled = true;
                }
                break;
            case State.Rising:
                timeElapsed += Time.deltaTime;
                if (timeElapsed < risingDuration)
                    SetSpikesScale(timeElapsed / risingDuration);
                else
                {
                    SetSpikesScale(1f);
                    timeElapsed = 0f;
                    state = State.Staying;
                }
                break;
            case State.Staying:
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= stayingDuration)
                {
                    timeElapsed = 0f;
                    state = State.Falling;
                }
                break;
            case State.Falling:
                timeElapsed += Time.deltaTime;
                if (timeElapsed < fallingDuration)
                {
                    telegraphVFX.transform.localScale = new(1f, 1f - timeElapsed / fallingDuration, 1f);
                    SetSpikesScale(1f - timeElapsed / fallingDuration);
                }
                else
                {
                    SetTelegraphScale(0f);
                    SetSpikesScale(0f);
                    state = State.Inactive;
                    collider.enabled = false;
                    spikesVFX.SetActive(false);
                    telegraphVFX.SetActive(false);
                    if (playerMovement != null)
                        playerMovement.slowingFactor = 1f;
                    playerMovement = null;
                }
                break;
        }
    }

    public bool IsInactive()
    {
        return state == State.Inactive;
    }

    private void OnTriggerEnter(Collider other)
    {
        TriggerCheck(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TriggerCheck(other);
    }

    private void TriggerCheck(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerMovement = other.GetComponent<PlayerMovement>();
        Assert.IsNotNull(playerMovement);
        playerMovement.slowingFactor = slowingFactor;

        Health health = other.GetComponent<Health>();
        Assert.IsNotNull(health);

        if (firstEntered)
        {
            firstEntered = false;
            health.TakeDamage(initialDamage);
        }
        else
        {
            damageDebt += damageOverTime * Time.deltaTime;
            health.TakeDamage((int)damageDebt);
            damageDebt -= (int)damageDebt;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        Assert.IsNotNull(playerMovement);
        playerMovement.slowingFactor = 1f;
        playerMovement = null;
    }

    public void Execute()
    {
        state = State.Telegraphing;
        collider.enabled = true;
        telegraphVFX.SetActive(true);
        telegraphVFX.transform.Rotate(0f, Random.value * 360f, 0f);
        SetTelegraphScale(0f);
        damageDebt = 0f;
        firstEntered = true;
    }

    private void SetTelegraphScale(float scale)
    {
        telegraphVFX.transform.localScale = new(scale, 1f, scale);
    }

    private void SetSpikesScale(float scale)
    {
        spikesVFX.transform.localScale = new(1f, scale, 1f);
    }
}
