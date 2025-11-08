using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LevelStastics
{
    private int finalHealth;
    private int totalCurrency;
    private int _lastCurrency;
    private readonly Dictionary<EnemyType, int> enemiesDefeated = new();
    private float startingTime;
    private float endingTime = 0f;

    public void Initialize()
    {
        Health health = Portal.GetPlayer().GetComponent<Health>();
        Assert.IsNotNull(health);
        finalHealth = health.GetCurrentHealth();
        health.onHealthChanged.AddListener((health, maxHealth) => finalHealth = health);

        PlayerCurrency currency = Portal.GetPlayer().GetComponent<PlayerCurrency>();
        Assert.IsNotNull(currency);
        totalCurrency = _lastCurrency = currency.GetCurrency();
        currency.onCurrencyChanged.AddListener(c => {
            totalCurrency += Math.Max(c - _lastCurrency, 0);
            _lastCurrency = c;
        });

        startingTime = Time.time;
    }

    public void AddEnemyDeath(EnemyType enemyType)
    {
        if (enemiesDefeated.ContainsKey(enemyType))
            ++enemiesDefeated[enemyType];
        else
            enemiesDefeated[enemyType] = 0;
    }

    public void StopTimer()
    {
        endingTime = Time.time;
    }

    public class Stats
    {
        public int finalHealth;
        public int totalCurrency;
        public Dictionary<EnemyType, int> enemiesDefeated;
        public float totalTime;

        public int GetEnemiesDefeated(EnemyType enemyType)
        {
            return enemiesDefeated.GetValueOrDefault(enemyType);
        }
    }

    public Stats GatherStats()
    {
        return new Stats() {
            finalHealth = finalHealth,
            totalCurrency = totalCurrency,
            enemiesDefeated = enemiesDefeated,
            totalTime = (endingTime != 0f ? endingTime : Time.time) - startingTime
        };
    }
}
