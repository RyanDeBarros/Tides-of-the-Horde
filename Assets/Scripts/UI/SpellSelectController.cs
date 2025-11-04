using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SpellSelectController : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Transform cooldown;
    [SerializeField] private TextMeshProUGUI keyHint;
    [SerializeField] private HUDController hud;
    [SerializeField] private float deselectedScale = 0.5f;
    [SerializeField] private SpellType spellType;

    private void Awake()
    {
        Assert.IsNotNull(image);
        Assert.IsNotNull(cooldown);
        Assert.IsNotNull(keyHint);
        ShowLocked();
        ShowDeselected();
    }

    public SpellType GetSpellType()
    {
        return spellType;
    }

    public void SetCooldown(float cooldown)
    {
        this.cooldown.localScale = new Vector3(1f, cooldown, 1f);
    }

    public void ShowSelected()
    {
        image.transform.localScale = Vector3.one;
        keyHint.gameObject.SetActive(false);
    }

    public void ShowDeselected()
    {
        image.transform.localScale = new Vector3(deselectedScale, deselectedScale, 1f);
        keyHint.gameObject.SetActive(true);
    }

    public void ShowLocked()
    {
        image.texture = null;
        SetCooldown(1f);
        gameObject.SetActive(false);
    }

    public void ShowUnlocked(int numberKey, SpellType spellType, Texture texture)
    {
        keyHint.SetText($"{numberKey}");
        this.spellType = spellType;
        image.texture = texture;
        SetCooldown(0f);
        gameObject.SetActive(true);
    }

    public bool IsUnlocked()
    {
        return gameObject.activeSelf;
    }

    public int GetNumberKey()
    {
        return gameObject.activeSelf ? int.Parse(keyHint.text) : -1;
    }
}
