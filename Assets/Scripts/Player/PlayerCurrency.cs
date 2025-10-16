using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField, Min(0)] private int currency = 0;
    public UnityEvent<int> onCurrencyChanged;  

    public int GetCurrency()
    {
        return currency;
    }

    public void Add(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currency += amount;
        onCurrencyChanged?.Invoke(currency);
    }

    public void Pay(int amount)
    {
        Assert.IsTrue(currency >= amount && amount >= 0);
        currency -= amount;
        onCurrencyChanged?.Invoke(currency);
    }

    // TODO Currency multiplier and currency bonus could be possible reward for NPC challenges
}
