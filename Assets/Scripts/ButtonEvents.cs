using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip clickSFX;

    void Awake()
    {
        if (audioSource == null)
        {
            // try find a global UI AudioSource in scene
            audioSource = FindFirstObjectByType<AudioSource>();
            if (audioSource != null)
                Debug.Log($"[ButtonSFX] Auto-assigned AudioSource from scene to '{gameObject.name}'");
        }

        if (audioSource == null)
            Debug.LogError($"[ButtonSFX] No AudioSource assigned or found for '{gameObject.name}'. Assign one in Inspector.");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource == null) return;
        if (hoverSFX == null)
        {
            Debug.LogWarning($"[ButtonSFX] hoverSFX is null on '{gameObject.name}'");
            return;
        }
        audioSource.PlayOneShot(hoverSFX);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (audioSource == null) return;
        if (clickSFX == null)
        {
            Debug.LogWarning($"[ButtonSFX] clickSFX is null on '{gameObject.name}'");
            return;
        }
        audioSource.PlayOneShot(clickSFX);
    }
}
