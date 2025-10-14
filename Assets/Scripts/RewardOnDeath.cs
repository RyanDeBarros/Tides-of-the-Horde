using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Health))]
public class RewardOnDeath : MonoBehaviour
{
    [Min(1)] public int reward = 10;
    [SerializeField] private string playerTag = "Player";

    private Health health;
    private PlayerCurrency playerCurrency;

    void Awake()
    {
        health = GetComponent<Health>();

        GameObject playerGO = GameObject.FindWithTag(playerTag);
        Assert.IsNotNull(playerGO);
        playerCurrency = playerGO.GetComponentInChildren<PlayerCurrency>();
        Assert.IsNotNull(playerCurrency);
    }

    void OnEnable()
    {
        health.onDeath.AddListener(GrantXP);
    }

    void OnDisable()
    {
        health.onDeath.RemoveListener(GrantXP);
    }

    private void GrantXP()
    {
        playerCurrency.AddCurrency(reward);
    }
}
