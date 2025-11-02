using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    [SerializeField] private Transform cooldown;
    [SerializeField] private PlayerDash dash;

    private void Awake()
    {
        Assert.IsNotNull(cooldown);
        Assert.IsNotNull(dash);
    }

    private void Update()
    {
        cooldown.localScale = new(1f, dash.GetNormalizedCooldown());
    }

    public void ShowUI()
    {
        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
