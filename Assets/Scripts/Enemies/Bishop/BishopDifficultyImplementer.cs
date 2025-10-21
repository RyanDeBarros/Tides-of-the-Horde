using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BishopDifficultyImplementer : MonoBehaviour, IDifficultyImplementer
{
    [System.Serializable]
    public class DifficultyStats
    {

    }

    [SerializeField] private List<DifficultyStats> stats;

    private int difficultyLevel = 0;

    private void Awake()
    {
    }

    private void Start()
    {
        // TODO apply difficulty level
    }

    public void SetDifficultyLevel(int level)
    {
        difficultyLevel = level;
    }
}
