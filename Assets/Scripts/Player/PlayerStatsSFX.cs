using UnityEngine;

public class PlayerStatsSFX : MonoBehaviour
{
    [SerializeField] private AudioClip takeDamageAudioClip;
    private int lastHealth = 0;

    [SerializeField] private AudioClip deathAudioClip;

    [SerializeField] private AudioClip gainCurrencyAudioClip;
    private int lastCurrency = 0;
    
    private void Start()
    {
        GameObject go = new("Stats SFX");
        go.transform.SetParent(transform, false);

        if (takeDamageAudioClip != null && TryGetComponent(out Health health))
        {
            AudioSource audioSource = go.AddComponent<AudioSource>();
            lastHealth = health.maxHealth;
            health.onHealthChanged.AddListener((h, m) => {
                if (h < lastHealth && h > 0)
                {
                    lastHealth = h;
                    audioSource.PlayOneShot(takeDamageAudioClip);
                }
            });
            health.onDeath.AddListener(() => audioSource.PlayOneShot(deathAudioClip));
        }

        if (gainCurrencyAudioClip != null && TryGetComponent(out PlayerCurrency currency))
        {
            AudioSource audioSource = go.AddComponent<AudioSource>();
            currency.onCurrencyChanged.AddListener(c => {
                if (c > lastCurrency)
                {
                    lastCurrency = c;
                    audioSource.PlayOneShot(gainCurrencyAudioClip);
                }
            });
        }
    }
}
