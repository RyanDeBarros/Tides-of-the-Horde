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
    [SerializeField] private string songIdentifier = "Worm";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip npcSFX;

    public UnityEvent onConversationEnd;

    private ChallengeGiverAnimator animator;

    private Coroutine spawnRoutine = null;
    private Coroutine despawnRoutine = null;
    private Transform player;
    private PlayerCamera playerCamera;

    private bool isTalking = false;

    private void Awake()
    {
        Assert.IsNotNull(keyHint);

        animator = GetComponentInChildren<ChallengeGiverAnimator>();
        Assert.IsNotNull(animator);

        player = GameObject.FindGameObjectWithTag(playerTag).transform;
        Assert.IsNotNull(player);
        playerCamera = player.GetComponent<PlayerCamera>();
        Assert.IsNotNull(playerCamera);

        Assert.IsNotNull(dialog);
        dialog.onClose = DespawnNPC;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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
                if (playerCamera.IsCameraEnabled() && Input.GetKeyDown(KeyCode.E))
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
        SoundtrackManager.Instance.PlayTrack(songIdentifier);
        spawnRoutine ??= StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (audioSource != null && npcSFX != null)
            audioSource.PlayOneShot(npcSFX);

        yield return animator.AnimateSpawn();
        keyHint.enabled = true;
    }

    private void DespawnNPC()
    {
        despawnRoutine ??= StartCoroutine(DespawnRoutine());
    }

    private IEnumerator DespawnRoutine()
    {
        yield return null;
        if (audioSource != null && npcSFX != null)
            audioSource.PlayOneShot(npcSFX);
        keyHint.enabled = false;
        yield return animator.AnimateDespawn();
        gameObject.SetActive(false);
        despawnRoutine = null;
        isTalking = false;
        onConversationEnd.Invoke();
    }
}
