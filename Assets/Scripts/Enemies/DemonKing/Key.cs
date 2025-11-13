using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Key : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float spawnDuration = 0.5f;
    [SerializeField] private float despawnDuration = 0.5f;
    public BossShield shield;

    [SerializeField] private AudioClip spawnSFX;
    [SerializeField] private AudioClip collectSFX;
    private AudioSource audioSource;

    private float timeElapsed = 0f;
    private bool despawning = false;
    private bool collected = false;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        Assert.IsNotNull(shield);
        StartCoroutine(Spawn());
    }

    private void Update()
    {
        if (!despawning)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= lifetime)
                StartCoroutine(Despawn());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Assert.IsTrue(other.CompareTag("Player"));
        StartCoroutine(Collect());
    }

    private IEnumerator Collect()
    {
        if (collected)
            yield break;
        collected = true;

        if (collectSFX)
            audioSource.PlayOneShot(collectSFX);

        yield return Despawn();
        shield.CollectKey();
    }

    private IEnumerator Despawn()
    {
        if (despawning)
            yield break;
        despawning = true;

        Vector3 initialScale = transform.localScale;

        for (float t = 0; t < despawnDuration; t += Time.deltaTime)
        {
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t / despawnDuration);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private IEnumerator Spawn()
    {
        if (spawnSFX)
            audioSource.PlayOneShot(spawnSFX);

        transform.localScale = Vector3.zero;

        for (float t = 0; t < spawnDuration; t += Time.deltaTime)
        {
            if (despawning)
                yield break;

            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / spawnDuration);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}
