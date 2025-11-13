using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class LevelStatistics
{
    private static int finalHealth;
    private static int maxHealth;
    private static int totalCurrency;
    private static int _lastCurrency;
    private static readonly Dictionary<EnemyType, int> enemiesDefeated = new();
    private static float startingTime;
    private static float endingTime = 0f;

    private static void Reset()
    {
        finalHealth = 0;
        maxHealth = 0;
        totalCurrency = 0;
        _lastCurrency = 0;
        enemiesDefeated.Clear();
        startingTime = 0f;
        endingTime = 0f;
    }

    public static void Initialize()
    {
        Reset();

        Health health = Portal.GetPlayer().GetComponent<Health>();
        Assert.IsNotNull(health);
        finalHealth = health.GetCurrentHealth();
        health.onHealthChanged.AddListener((health, maxHealth) => { finalHealth = health; LevelStatistics.maxHealth = maxHealth; });

        PlayerCurrency currency = Portal.GetPlayer().GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);
        totalCurrency = _lastCurrency = currency.GetCurrency();
        currency.onCurrencyChanged.AddListener(c => {
            totalCurrency += Math.Max(c - _lastCurrency, 0);
            _lastCurrency = c;
        });

        startingTime = Time.time;
    }

    public static void AddEnemyDeath(EnemyType enemyType)
    {
        if (enemiesDefeated.ContainsKey(enemyType))
            ++enemiesDefeated[enemyType];
        else
            enemiesDefeated[enemyType] = 0;
    }

    public static void StopTimer()
    {
        endingTime = Time.time;
    }

    public static int GetFinalHealth()
    {
        return finalHealth;
    }

    public static int GetMaxHealth()
    {
        return maxHealth;
    }

    public static int GetTotalCurrency()
    {
        return totalCurrency;
    }

    public static float GetTotalTime()
    {
        return endingTime - startingTime;
    }

    public static int GetEnemiesDefeated(EnemyType enemyType)
    {
        return enemiesDefeated.GetValueOrDefault(enemyType);
    }
}
