using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class SpawnWaveUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveNumberText;
    [SerializeField] private Transform spawningTimeLeft;
    [SerializeField] private Transform waitTime;

    private void Awake()
    {
        Assert.IsNotNull(waveNumberText);
        Assert.IsNotNull(spawningTimeLeft);
        Assert.IsNotNull(waitTime);
    }

    public void SetWaveNumber(int waveNumber)
    {
        waveNumberText.SetText($"Wave {waveNumber}");
    }

    public void SetNormalizedSpawningTimeLeft(float timeLeft)
    {
        spawningTimeLeft.localScale = new(Mathf.Clamp01(timeLeft), 1f, 1f);
    }

    public void SetNormalizedWaitTime(float time)
    {
        waitTime.localScale = new(1f, Mathf.Clamp01(time), 1f);
    }

    public void HideUI()
    {
        waveNumberText.gameObject.SetActive(false);
        spawningTimeLeft.gameObject.SetActive(false);
        waitTime.gameObject.SetActive(false);
    }
}
