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
}
