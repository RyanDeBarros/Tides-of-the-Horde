using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

class SpikeTrap : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private new CapsuleCollider collider;
    [SerializeField] private GameObject spikesVFX;
    [SerializeField] private GameObject telegraphVFX;

    private int damage = 0;
    // TODO slow down player?

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
    private float telegraphDuration = 0f;
    private float risingDuration = 0f;
    private float stayingDuration = 0f;
    private float fallingDuration = 0f;

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

        // TODO can hit player multiple times if player stands in it? Damage over time or refreshes playerhit every X milliseconds?
        collider.enabled = false;

        Health health = other.GetComponent<Health>();
        Assert.IsNotNull(health);
        health.TakeDamage(damage);
    }

    public void Execute(float telegraphDuration, float risingDuration, float stayingDuration, float fallingDuration, int damage)
    {
        this.telegraphDuration = telegraphDuration;
        this.risingDuration = risingDuration;
        this.stayingDuration = stayingDuration;
        this.fallingDuration = fallingDuration;
        this.damage = damage;

        state = State.Telegraphing;
        collider.enabled = true;
        telegraphVFX.SetActive(true);
        telegraphVFX.transform.Rotate(0f, Random.value * 360f, 0f);
        SetTelegraphScale(0f);
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
