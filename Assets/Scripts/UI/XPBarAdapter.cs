using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

// TODO don't use bar
[RequireComponent(typeof(Slider))]
[DefaultExecutionOrder(1000)] 
public class SimpleXPBar : MonoBehaviour
{
    public PlayerCurrency currency;
    public TextMeshProUGUI xpText;

    [Min(1)] public int goal = 100;   
    public bool autoExpand = true;    
    public int expandStep = 100;      

    Slider s;

    void Awake()
    {
        s = GetComponent<Slider>();
        s.minValue = 0;
        s.maxValue = goal;
        s.wholeNumbers = true; 

        
        if (s.fillRect)
        {
            var f = s.fillRect;
            f.anchorMin = Vector2.zero;
            f.anchorMax = Vector2.one;
            f.offsetMin = Vector2.zero;
            f.offsetMax = Vector2.zero;
        }

        if (!currency)
        {
            GameObject player = GameObject.FindWithTag("Player");
            Assert.IsNotNull(player);
            currency = player.GetComponentInChildren<PlayerCurrency>();
            Assert.IsNotNull(currency);
        }

        currency.onCurrencyChanged.AddListener(UpdateBar);
        UpdateBar(currency.GetCurrency()); 
    }

    void OnDisable()
    {
        currency.onCurrencyChanged.RemoveListener(UpdateBar);
    }

    void LateUpdate()
    {
        
        if (!autoExpand && s.maxValue != goal) s.maxValue = goal;
    }

    void UpdateBar(int total)
    {
        if (autoExpand && total > s.maxValue)
        {
            float newMax = Mathf.Ceil(total / (float)expandStep) * expandStep;
            s.maxValue = Mathf.Max(newMax, goal);
        }

        s.value = Mathf.Clamp(total, 0, s.maxValue);
        if (xpText) xpText.text = $"{total}/{(int)s.maxValue}";
    }
}
