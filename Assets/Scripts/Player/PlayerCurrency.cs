using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField, Min(0)] private int currency = 0;
    [SerializeField, Min(0f)] private float currencyCollectMultiplier = 1f;
    [SerializeField, Range(0f, 1f)] private float shopDiscount = 0f;
    public UnityEvent<int> onCurrencyChanged;

    private void Start()
    {
        currency += PersistentChallengeData.Data().StartingBonus;
        shopDiscount += PersistentChallengeData.Data().ShopDiscount * (1f - shopDiscount);
        currencyCollectMultiplier += PersistentChallengeData.Data().CurrencyBoost;
        CurrencyChanged();
    }

    public int GetCurrency()
    {
        return currency;
    }

    public void Add(int amount)
    {
        Assert.IsTrue(amount >= 0);
        currency += (int)(amount * currencyCollectMultiplier);
        CurrencyChanged();
    }

    public void Pay(int amount)
    {
        Assert.IsTrue(currency >= amount && amount >= 0);
        currency -= amount;
        CurrencyChanged();
    }

    public float GetMultiplier()
    {
        return currencyCollectMultiplier;
    }

    public void SetMultiplier(float multiplier)
    {
        currencyCollectMultiplier = Mathf.Max(multiplier, 0f);
    }

    public int ShopPrice(int originalPrice)
    {
        return Mathf.CeilToInt(originalPrice * (1f - shopDiscount));
    }

    private void CurrencyChanged()
    {
        onCurrencyChanged?.Invoke(currency);
    }
}
