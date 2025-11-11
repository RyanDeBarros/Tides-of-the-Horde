using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Key : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float collectDespawnDuration = 0.5f;
    public BossShield shield;

    private float timeElapsed = 0f;
    private bool despawning = false;
    private bool collected = false;

    private void Start()
    {
        Assert.IsNotNull(shield);
        // TODO spawn coroutine
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
        // TODO play SFX

        yield return Despawn();
        shield.CollectKey();
    }

    private IEnumerator Despawn()
    {
        if (despawning)
            yield break;
        despawning = true;

        transform.localScale = Vector3.one;

        for (float t = 0; t < collectDespawnDuration; t += Time.deltaTime)
        {
            float a = 1f - t / collectDespawnDuration;
            transform.localScale = new Vector3(a, a, a);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}
