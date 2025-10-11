using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public enum EnemyType
{
    Skeleton,
    Bishop,
}

[System.Serializable]
public class WaveTimeline
{
    [System.Serializable]
    private class Subwave : ISerializationCallbackReceiver
    {
        public float timeOffset = 0f;
        public float duration;
        [SerializeField] private string enemy;
        public EnemyType type;
        public float spawnRate;

        private float spawnDebt = 0f;
        private float elapsed = 0f;
        private int toSpawn = 0;

        public void OnAfterDeserialize()
        {
            if (enemy == EnemyType.Skeleton.ToString())
                type = EnemyType.Skeleton;
            else if (enemy == EnemyType.Bishop.ToString())
                type = EnemyType.Bishop;
            else
                throw new Exception($"Enemy \"{enemy}\" does not correspond to a known enemy type.");
        }

        public void OnBeforeSerialize() { }

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

        public int GetNumberToSpawn()
        {
            return toSpawn;
        }
    }

    [System.Serializable]
    private class Wave : ISerializationCallbackReceiver
    {
        public float preWaveWaitTime;
        public float postWaveWaitTime;
        public List<Subwave> subwaves;

        private float fullSpawnDuration;

        public void OnAfterDeserialize()
        {
            fullSpawnDuration = subwaves.Count > 0 ? subwaves.Select(subwave => subwave.timeOffset + subwave.duration).Max() : 0f;
        }

        public void OnBeforeSerialize() { }

        public float FullSpawnDuration()
        {
            return fullSpawnDuration;
        }
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
                    TransitionToSpawning();
                break;
            case WaveState.Spawning:
                ProcessSubwaves();
                if (waveTimeElapsed > wave.FullSpawnDuration())
                    TransitionToPostSpawn();
                break;
            case WaveState.PostSpawn:
                if (waveTimeElapsed > wave.postWaveWaitTime)
                    TransitionToPreSpawn();
                break;
        }
    }

    private void TransitionToSpawning()
    {
        Wave wave = waves[waveNumber];
        waveState = wave.subwaves.Count > 0 ? WaveState.Spawning : WaveState.PostSpawn;
        waveTimeElapsed -= wave.preWaveWaitTime;
        SyncTimeline();
    }

    private void ProcessSubwaves()
    {
        waves[waveNumber].subwaves.ForEach(subwave => {
            subwave.Sync(waveTimeElapsed);
            if (toSpawn.ContainsKey(subwave.type))
                toSpawn[subwave.type] += subwave.GetNumberToSpawn();
            else
                toSpawn[subwave.type] = subwave.GetNumberToSpawn();
        });
    }

    private void TransitionToPostSpawn()
    {
        waveState = WaveState.PostSpawn;
        waveTimeElapsed -= waves[waveNumber].FullSpawnDuration();
        SyncTimeline();
    }

    private void TransitionToPreSpawn()
    {
        waveState = WaveState.PreSpawn;
        waveTimeElapsed -= waves[waveNumber].postWaveWaitTime;
        ++waveNumber;
        SyncTimeline();
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
