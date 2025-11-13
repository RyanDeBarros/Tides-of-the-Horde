using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    public static SoundtrackManager Instance { get; private set; }

    [Serializable]
    private class Track
    {
        public string identifier;
        public AudioClip clip;
        public bool loop = true;
        public float volume = 1f;
    }

    [SerializeField] private List<Track> trackList;
    [SerializeField, Min(0f)] private float crossFadeDuration = 0.5f;
    [SerializeField, Range(0f, 1f)] private float dimmedVolumeFactor = 0.5f;
    [SerializeField, Min(0f)] private float dimmedTransitionDuration = 0.3f;
    [SerializeField, Min(2)] private int numberOfSources = 3;

    private Dictionary<string, Track> tracks;

    private class TrackCache
    {
        public float lastTime = 0f;
        public float position = 0f;
    }

    private readonly Dictionary<string, TrackCache> trackCache = new();
    
    private AudioSource[] sources;
    private int activeSourceIndex = 0;
    private string activeIdentifier;
    private Coroutine dimCoroutine;
    private bool dimmed = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (transform.parent != null)
            transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        tracks = trackList.ToDictionary(t => t.identifier);
        sources = new AudioSource[numberOfSources];
        for (int i = 0; i < numberOfSources; ++i)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
            sources[i].volume = 0f;
        }

        trackList.ForEach(t => StartCoroutine(PrepareClip(t.clip)));
    }

    private IEnumerator PrepareClip(AudioClip track)
    {
        track.LoadAudioData();
        while (track.loadState == AudioDataLoadState.Loading)
            yield return null;
    }

    public void PlayTrack(string identifier, bool restart = true)
    {
        if (!tracks.TryGetValue(identifier, out Track track))
        {
            Debug.LogWarning($"Unrecognized song identifier \"{identifier}\"");
            return;
        }

        AudioSource fromSource = sources[activeSourceIndex];
        if (fromSource.clip == track.clip) return;

        activeSourceIndex = (activeSourceIndex + 1) % sources.Length;
        AudioSource toSource = sources[activeSourceIndex];
        toSource.clip = track.clip;
        toSource.loop = track.loop;

        if (!restart)
            toSource.time = Time.time - trackCache[identifier].lastTime + trackCache[identifier].position;

        StartCoroutine(PrepareClip(toSource.clip));
        if (dimCoroutine != null)
            StopCoroutine(dimCoroutine);
        dimmed = false;
        StartCoroutine(FadeOut(fromSource, activeIdentifier));
        activeIdentifier = identifier;
        StartCoroutine(FadeIn(toSource, track.volume));
    }

    private IEnumerator FadeOut(AudioSource source, string identifier)
    {
        float fromVolume = source.volume;
        for (float t = 0f; t < crossFadeDuration; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Clamp01(1f - t / crossFadeDuration) * fromVolume;
            yield return null;
        }

        source.volume = 0f;
        if (identifier != null)
            trackCache[identifier] = new() { lastTime = Time.time, position = source.time };
        source.Stop();
    }

    private IEnumerator FadeIn(AudioSource source, float toVolume)
    {
        source.volume = 0f;
        source.Play();

        for (float t = 0f; t < crossFadeDuration; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Clamp01(t / crossFadeDuration) * toVolume;
            yield return null;
        }

        source.volume = toVolume;
    }

    public void DimTrack()
    {
        if (dimmed) return;
        dimmed = true;

        if (dimCoroutine != null)
            StopCoroutine(dimCoroutine);
        AudioSource source = sources[activeSourceIndex];
        dimCoroutine = StartCoroutine(DimTrackRoutine(source, source.volume, dimmedVolumeFactor * tracks[activeIdentifier].volume));
    }

    public void UnDimTrack()
    {
        if (!dimmed) return;
        dimmed = false;

        if (dimCoroutine != null)
            StopCoroutine(dimCoroutine);
        AudioSource source = sources[activeSourceIndex];
        dimCoroutine = StartCoroutine(DimTrackRoutine(source, source.volume, tracks[activeIdentifier].volume));
    }

    private IEnumerator DimTrackRoutine(AudioSource source, float fromVolume, float toVolume)
    {
        source.volume = fromVolume;
        for (float t = 0f; t < dimmedTransitionDuration; t += Time.unscaledDeltaTime)
        {
            source.volume = Mathf.Lerp(fromVolume, toVolume, Mathf.Clamp01(t / dimmedTransitionDuration));
            yield return null;
        }

        source.volume = toVolume;
        dimCoroutine = null;
    }
}
