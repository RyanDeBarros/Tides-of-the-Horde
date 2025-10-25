using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ChallengeGiver : MonoBehaviour
{
    public UnityEvent onConversationEnd;

    private ChallengeGiverAnimator animator;

    private Coroutine despawnRoutine = null;

    private void Awake()
    {
        animator = GetComponentInChildren<ChallengeGiverAnimator>();
        Assert.IsNotNull(animator);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        // TODO always face player

        if (Input.GetKeyDown(KeyCode.E))
        {
            // TODO start conversation if near player
            DespawnNPC();
        }
    }

    public void SpawnNPC()
    {
        gameObject.SetActive(true);
        animator.AnimateSpawn();
    }

    private void DespawnNPC()
    {
        despawnRoutine ??= StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        yield return animator.AnimateDespawn();
        gameObject.SetActive(false);
        despawnRoutine = null;
        onConversationEnd.Invoke();
    }
}
