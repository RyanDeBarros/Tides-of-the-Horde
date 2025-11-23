using UnityEngine;

public class PlayerStatsSFX : MonoBehaviour
{
    [SerializeField] private AudioClip takeDamageAudioClip;
    [SerializeField] private AudioClip deathAudioClip;
    [SerializeField] private AudioClip gainCurrencyAudioClip;
    [SerializeField] private AudioClip shopBuyAudioClip;
    [SerializeField] private AudioClip openShopAudioClip;
    [SerializeField] private AudioClip closeShopAudioClip;

    // Two independent audio sources for health sfx and currency sfx
    private AudioSource healthAudioSource;
    private AudioSource currencyAudioSource;
    
    private void Start()
    {
        GameObject go = new("Stats SFX");
        go.transform.SetParent(transform, false);
        healthAudioSource = go.AddComponent<AudioSource>();
        currencyAudioSource = go.AddComponent<AudioSource>();

        if (deathAudioClip != null && TryGetComponent(out Health health))
            health.onDeath.AddListener(() => healthAudioSource.PlayOneShot(deathAudioClip));
    }

    public void PlayTakeDamageSFX()
    {
        if (takeDamageAudioClip != null)
            currencyAudioSource.PlayOneShot(takeDamageAudioClip);
    }

    public void PlayGainCurrencySFX()
    {
        if (gainCurrencyAudioClip != null)
            currencyAudioSource.PlayOneShot(gainCurrencyAudioClip);
    }

    public void PlayPurchaseSFX()
    {
        if (shopBuyAudioClip != null)
            currencyAudioSource.PlayOneShot(shopBuyAudioClip);
    }

    public void PlayShopOpen()
    {
        if (openShopAudioClip != null)
            currencyAudioSource.PlayOneShot(openShopAudioClip);
    }

    public void PlayShopClose()
    {
        if (closeShopAudioClip != null)
            currencyAudioSource.PlayOneShot(closeShopAudioClip);
    }
}
