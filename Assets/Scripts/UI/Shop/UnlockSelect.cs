using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class UnlockSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private RawImage icon;
    [SerializeField] private GameObject availableInfoRoot;

    private PlayerUnlockNode node = null;
    private PlayerCurrency playerCurrency;

    private void Awake()
    {
        Assert.IsNotNull(nameText);
        Assert.IsNotNull(descriptionText);
        Assert.IsNotNull(costText);
        Assert.IsNotNull(icon);
        Assert.IsNotNull(availableInfoRoot);

        playerCurrency = GlobalFind.FindUniqueObjectByType<PlayerCurrency>(true);
    }

    public void Setup(PlayerUnlockNode node)
    {
        this.node = node;
        nameText.SetText(node.GetName());
        descriptionText.SetText(node.GetDescription());
        costText.SetText($"{node.GetCost()}");
        icon.texture = node.GetIconTexture();
    }

    public void OnClick()
    {
        if (node != null)
        {
            if (playerCurrency.GetCurrency() >= node.GetCost())
            {
                playerCurrency.Pay(node.GetCost());
                node.Activate();
                ShowUnavailable();
            }
        }
    }

    public void ShowAvailable()
    {
        availableInfoRoot.SetActive(true);
    }

    public void ShowUnavailable()
    {
        availableInfoRoot.SetActive(false);
        node = null;
    }
}
