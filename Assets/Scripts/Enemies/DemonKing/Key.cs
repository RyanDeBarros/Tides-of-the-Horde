using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class Key : MonoBehaviour
{
    [SerializeField] private float collectDespawnDuration = 0.5f;

    private Coroutine collectRoutine = null;

    private void OnTriggerEnter(Collider other)
    {
        Assert.IsTrue(other.CompareTag("Player"));
        collectRoutine ??= StartCoroutine(Collect());
    }

    private IEnumerator Collect()
    {
        transform.localScale = Vector3.one;

        for (float t = 0; t < collectDespawnDuration; t += Time.deltaTime)
        {
            float a = 1f - t / collectDespawnDuration;
            transform.localScale = new Vector3(a, a, a);
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
        // TODO increment player's collectible count
    }
}
