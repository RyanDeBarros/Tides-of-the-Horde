using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Skeleton,
    Bishop,
}

[System.Serializable]
public class WaveTimeline
{
    [System.Serializable]
    private class Subwave
    {
        public float timeOffset = 0f;
        public float duration;
        public EnemyType type;
        public float spawnRate;

        private float spawnDebt = 0f;
        private float elapsed = 0f;
        private int toSpawn = 0;

        public void Sync(float waveTimeElapsed)
        {
            if (waveTimeElapsed > timeOffset)
            {
                float elapsedSinceOffset = Mathf.Min(waveTimeElapsed - timeOffset, duration);
                float delta = elapsedSinceOffset - elapsed;
                if (delta > 0f)
                {
                    elapsed = elapsedSinceOffset;
                    toSpawn = (int)((delta + spawnDebt) * spawnRate);
                    spawnDebt += delta - toSpawn / spawnRate;
                }
                else
                    toSpawn = 0;
            }
            else
                toSpawn = 0;
        }

        public bool Finished()
        {
            return elapsed >= duration;
        }

        public int GetNumberToSpawn()
        {
            return toSpawn;
        }
    }

    [System.Serializable]
    private class Wave
    {
        public float preWaveWaitTime;
        public float postWaveWaitTime;
        public List<Subwave> subwaves;
    }

    private enum WaveState
    {
        PreSpawn,
        Spawning,
        PostSpawn
    }

    [SerializeField] private List<Wave> waves;
    private int waveNumber = 0;
    private float waveTimeElapsed = 0f;
    private WaveState waveState = WaveState.PreSpawn;
    private readonly Dictionary<EnemyType, int> toSpawn = new();

    public static WaveTimeline Read(TextAsset file)
    {
        return JsonUtility.FromJson<WaveTimeline>(file.text);
    }

    public void ManualUpdate()
    {
        toSpawn.Clear();
        waveTimeElapsed += Time.deltaTime;
        SyncTimeline();
    }

    private void SyncTimeline()
    {
        if (waveNumber >= waves.Count)
            return;

        Wave wave = waves[waveNumber];
        switch (waveState)
        {
            case WaveState.PreSpawn:
                if (waveTimeElapsed > wave.preWaveWaitTime)
                {
                    // Finish initial waiting -> start wave
                    waveState = WaveState.Spawning;
                    waveTimeElapsed -= wave.preWaveWaitTime;
                    SyncTimeline();
                }
                break;
            case WaveState.Spawning:
                if (wave.subwaves.Count > 0)
                {
                    bool finishWave = true;
                    float fullDuration = 0f;
                    wave.subwaves.ForEach(subwave => {
                        subwave.Sync(waveTimeElapsed); // Sync subwave data
                        if (!subwave.Finished()) // Check if subwave can continue spawning
                            finishWave = false;
                        else
                            fullDuration = Mathf.Max(fullDuration, subwave.timeOffset + subwave.duration);
                        if (toSpawn.ContainsKey(subwave.type)) // Increase the number of enemies to spawn for this frame
                            toSpawn[subwave.type] += subwave.GetNumberToSpawn();
                        else
                            toSpawn[subwave.type] = subwave.GetNumberToSpawn();
                    });
                    if (finishWave)
                    {
                        // Finish wave -> start closing wait time
                        waveState = WaveState.PostSpawn;
                        waveTimeElapsed -= fullDuration;
                        SyncTimeline();
                    }
                }
                else
                {
                    waveState = WaveState.PostSpawn;
                    SyncTimeline();
                }
                break;
            case WaveState.PostSpawn:
                if (waveTimeElapsed > wave.postWaveWaitTime)
                {
                    // Finish wave -> move to next wave
                    waveState = WaveState.PreSpawn;
                    waveTimeElapsed -= wave.postWaveWaitTime;
                    ++waveNumber;
                    SyncTimeline();
                }
                break;
        }
    }

    public int WaveNumber() // TODO use in UI
    {
        return waveNumber;
    }

    public Dictionary<EnemyType, int> GetEnemiesToSpawn()
    {
        return toSpawn;
    }
}
