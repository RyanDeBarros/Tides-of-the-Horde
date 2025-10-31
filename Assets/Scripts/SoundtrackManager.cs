using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance;

    [Serializable]
    private class Track
    {
        public string identifier;
        public AudioClip clip;
        public bool loop = true;
        public float volume = 1f;
    }

    [SerializeField] private List<Track> trackList;
    [SerializeField] private float crossFadeDuration = 0.4f;
    [SerializeField, Min(2)] private int numberOfSources = 3;

    private Dictionary<string, Track> tracks;
    private AudioSource[] sources;
    private int activeSourceIndex = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        tracks = trackList.ToDictionary(t => t.identifier);
        sources = new AudioSource[numberOfSources];
        for (int i = 0; i < numberOfSources; ++i)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }
    }

    public void PlayTrack(string identifier)
    {
        if (!tracks.TryGetValue(identifier, out Track track)) return;

        AudioSource fromSource = sources[activeSourceIndex];
        if (fromSource.clip == track.clip) return;

        ++activeSourceIndex;
        AudioSource toSource = sources[activeSourceIndex];
        toSource.clip = track.clip;
        toSource.loop = track.loop;

        StartCoroutine(FadeOut(fromSource));
        StartCoroutine(FadeIn(toSource, track.volume));
    }

    private IEnumerator FadeOut(AudioSource source)
    {
        float fromVolume = source.volume;
        for (float t = 0f; t < crossFadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Clamp01(1f - t / crossFadeDuration) * fromVolume;
            yield return null;
        }

        source.Stop();
    }

    private IEnumerator FadeIn(AudioSource source, float toVolume)
    {
        source.volume = 0f;
        source.Play();

        for (float t = 0f; t < crossFadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Clamp01(t / crossFadeDuration) * toVolume;
            yield return null;
        }

        source.volume = toVolume;
    }
}
