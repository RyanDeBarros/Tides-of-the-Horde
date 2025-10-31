using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class ChallengeGiver : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private NPCDialog dialog;
    [SerializeField] private float talkDistance = 10f;
    [SerializeField] private TextMeshPro keyHint;
    [SerializeField] private Color keyHintActiveColor;
    [SerializeField] private Color keyHintInactiveColor;

    public UnityEvent onConversationEnd;

    private ChallengeGiverAnimator animator;

    private Coroutine spawnRoutine = null;
    private Coroutine despawnRoutine = null;
    private Transform player;

    private bool isTalking = false;

    private void Awake()
    {
        Assert.IsNotNull(keyHint);

        animator = GetComponentInChildren<ChallengeGiverAnimator>();
        Assert.IsNotNull(animator);

        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        Assert.IsNotNull(player);

        Assert.IsNotNull(dialog);
        dialog.onClose = DespawnNPC;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        keyHint.enabled = false;
    }

    private void Update()
    {
        transform.LookAt(player.position);

        if (keyHint.enabled)
        {
            if (Vector3.Distance(transform.position, player.position) < talkDistance)
            {
                keyHint.color = keyHintActiveColor;
                if (Input.GetKeyDown(KeyCode.E))
                    Talk();
            }
            else
                keyHint.color = keyHintInactiveColor;
        }
    }

    private void Talk()
    {
        if (isTalking) return;

        isTalking = true;
        keyHint.enabled = false;
        dialog.Open();
    }

    public void SpawnNPC()
    {
        gameObject.SetActive(true);
        spawnRoutine ??= StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        // TODO play SFX
        yield return animator.AnimateSpawn();
        keyHint.enabled = true;
    }

    private void DespawnNPC()
    {
        despawnRoutine ??= StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        // TODO play SFX
        keyHint.enabled = false;
        yield return animator.AnimateDespawn();
        gameObject.SetActive(false);
        despawnRoutine = null;
        isTalking = false;
        onConversationEnd.Invoke();
    }
}
