using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField, Min(0)] private int currency = 0;
    [SerializeField, Min(1f)] private float currencyCollectMultiplier = 1f;
    [SerializeField, Range(0f, 1f)] private float shopDiscount = 0f;
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

    public void AddMultiplier(float boost)
    {
        currencyCollectMultiplier += boost;
    }

    public void CompoundShopDiscount(float discount)
    {
        discount = Mathf.Clamp01(discount);
        shopDiscount += discount * (1f - shopDiscount);
    }

    public int ShopPrice(int originalPrice)
    {
        return Mathf.CeilToInt(originalPrice * (1f - shopDiscount));
    }
}
