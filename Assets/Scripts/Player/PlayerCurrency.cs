using UnityEngine;
using UnityEngine.Events;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField, Min(0)] private int currency = 0;
    public UnityEvent<int> onCurrencyChanged;  

    public void AddCurrency(int amount)
    {
        if (amount <= 0) return;
        currency += amount;
        onCurrencyChanged?.Invoke(currency);
    }

    public int GetCurrency()
    {
        return currency;
    }
}
