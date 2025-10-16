using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField, Min(0)] private int currency = 0;
    [SerializeField, Min(1)] private float currencyCollectMultiplier = 1f;
    public UnityEvent<int> onCurrencyChanged;  

    public int GetCurrency()
    {
        return currency;
    }

    public void Add(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currency += (int)(amount * currencyCollectMultiplier);
        onCurrencyChanged?.Invoke(currency);
    }

    public void Pay(int amount)
    {
        Assert.IsTrue(currency >= amount && amount >= 0);
        currency -= amount;
        onCurrencyChanged?.Invoke(currency);
    }

    // TODO NPC challenge ideas:
    // Challenges:
    // - Defeat X# of enemies of some type and with some spell within time limit
    // - Survive wave without losing more than X health
    // - Defeat X# of enemies without moving
    // Rewards:
    // - Currency bonus
    // - Currency collect multiplier increase
    // - Shop discounts
}
