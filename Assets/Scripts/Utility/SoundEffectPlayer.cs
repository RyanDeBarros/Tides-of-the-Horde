using UnityEngine;

public class SoundEffectPlayer : MonoBehaviour
{
    private AudioClip clip;
    private AudioSource source;

    public static void Play(AudioClip clip)
    {
        new GameObject("SoundEffectPlayer").AddComponent<SoundEffectPlayer>().clip = clip;
    }

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        source.PlayOneShot(clip);
        Destroy(gameObject, clip.length);
    }
}
