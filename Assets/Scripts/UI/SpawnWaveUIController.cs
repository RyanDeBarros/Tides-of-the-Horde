using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnWaveUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveNumberText;

    private void Awake()
    {
        Assert.IsNotNull(waveNumberText);
    }

    public void SetWaveNumber(int waveNumber)
    {
        waveNumberText.SetText($"Wave {waveNumber}");
    }
}
