using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton
}

[System.Serializable]
public class WaveTimeline
{
    [System.Serializable]
    public class Subwave
    {
        public float timeOffset = 0f;
        public float duration;
        public EnemyType type;
        public float spawnRate;
    }

    [System.Serializable]
    public class Wave
    {
        public float preWaveWaitTime;
        public float postWaveWaitTime;
        public List<Subwave> subwaves;
    }

    public List<Wave> waves;

    public static WaveTimeline Read(TextAsset file)
    {
        return JsonUtility.FromJson<WaveTimeline>(file.text);
    }
}
