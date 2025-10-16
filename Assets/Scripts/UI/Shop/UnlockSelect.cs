using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class UnlockSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject availableInfoRoot;

    private PlayerUnlockNode node = null;
    private PlayerCurrency playerCurrency;

    private void Awake()
    {
        Assert.IsNotNull(nameText);
        Assert.IsNotNull(descriptionText);
        Assert.IsNotNull(costText);
        Assert.IsNotNull(availableInfoRoot);

        playerCurrency = FindObjectsByType<PlayerCurrency>(FindObjectsSortMode.None).GetUniqueElement();
    }

    public void Setup(PlayerUnlockNode node)
    {
        this.node = node;
        nameText.SetText(node.GetName());
        descriptionText.SetText(node.GetDescription());
        costText.SetText($"{node.GetCost()}");
        // TODO icon
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
        // TODO
        availableInfoRoot.SetActive(true);
    }

    public void ShowUnavailable()
    {
        // TODO
        availableInfoRoot.SetActive(false);
        node = null;
    }
}
