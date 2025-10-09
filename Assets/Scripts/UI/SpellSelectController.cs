using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class SpellSelectController : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Texture texture;
    [SerializeField] private Transform cooldown;
    [SerializeField] private TextMeshProUGUI keyHint;
    [SerializeField] private float deselectedScale = 0.5f;
    [SerializeField] private SpellType spellType;


    private void Awake()
    {
        Assert.IsNotNull(image);
        Assert.IsNotNull(cooldown);
        Assert.IsNotNull(texture);
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
    }

    public void ShowUnlocked()
    {
        image.texture = texture;
        SetCooldown(0f);
    }

    public void SetKeyHint(int number)
    {
        keyHint.SetText(number.ToString());
    }
}
