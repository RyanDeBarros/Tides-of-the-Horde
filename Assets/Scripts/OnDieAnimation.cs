using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class OnDieAnimation : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Animator anim;

    [SerializeField] private string dieStateName = "Die";
    [SerializeField] private float extraDelay = 0.15f;
    [SerializeField] private Behaviour[] disableOnDeath;

    [Header("Sink Settings")]
    [SerializeField] private float sinkDuration = 1.5f;
    [SerializeField] private float sinkSpeed = 0.6f;

    private void Awake()
    {
        if (health == null)
            health = GetComponent<Health>();
        Assert.IsNotNull(health);
        health.onDeath.AddListener(OnDie);

        if (anim == null)
            anim = GetComponentInChildren<Animator>();
    }

    private void OnDie()
    {
        DisableComponent<NavMeshAgent>(c => c.enabled = false);
        DisableComponent<CharacterController>(c => c.enabled = false);
        DisableComponent<Collider>(c => c.enabled = false);
        foreach (var c in disableOnDeath) c.enabled = false;

        if (anim)
            StartCoroutine(PlayDieThenSink());
        else
            StartCoroutine(SinkAndDestroy());
    }

    private void DisableComponent<T>(Action<T> disable)
    {
        if (TryGetComponent(out T component))
            disable(component);

        T[] components = GetComponentsInChildren<T>();
        foreach (T c in components)
            disable(c);
    }

    private IEnumerator PlayDieThenSink()
    {
        anim.SetTrigger(dieStateName);

        float len = 0f;
        var rc = anim.runtimeAnimatorController;
        if (rc != null)
        {
            var clip = rc.animationClips.FirstOrDefault(clip => clip.name == dieStateName);
            if (clip != null) len = clip.length;
        }
        if (len <= 0f) len = 2f;

        yield return new WaitForSeconds(len + extraDelay);
        yield return SinkAndDestroy();
    }

    private IEnumerator SinkAndDestroy()
    {
        float t = 0f;
        while (t < sinkDuration)
        {
            transform.Translate(sinkSpeed * Time.deltaTime * Vector3.down, Space.World);
            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
