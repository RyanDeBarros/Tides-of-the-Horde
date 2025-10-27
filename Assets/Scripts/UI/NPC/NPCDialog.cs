using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

[Serializable]
public class DialogOption
{
    public string text;
    public bool clickable = false;
    public float topPadding = 0f;
    public float halign = 0f;
}

[SerializeField]
public class DialogPage
{
    public List<DialogOption> options;
}

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private RectTransform textArea;
    [SerializeField] private TextAsset testDialog;

    [Header("Text Parameters")]
    [SerializeField] private float typingSeconds = 0.02f;
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float fontSize = 24;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color textHoverColor = Color.blue;
    [SerializeField] private TMP_FontAsset font;

    public Action onClose;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerMovement playerMovement;

    private bool open = false;

    private readonly List<TextMeshProUGUI> textComponents = new();
    private readonly List<Button> textButtons = new();
    private readonly List<OnHover> textOnHovers = new();
    private CoroutineQueue typewriterAnimationQueue;

    private void Awake()
    {
        Assert.IsNotNull(textArea);
        Assert.IsNotNull(font);

        camera = FindObjectsByType<PlayerCamera>(FindObjectsSortMode.None).GetUniqueElement();
        spellManager = FindObjectsByType<SpellManager>(FindObjectsSortMode.None).GetUniqueElement();
        playerMovement = FindObjectsByType<PlayerMovement>(FindObjectsSortMode.None).GetUniqueElement();

        typewriterAnimationQueue = gameObject.AddComponent<CoroutineQueue>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (open && Input.GetKey(KeyCode.Escape))
            Close();
    }

    public void Open()
    {
        if (open) return;

        open = true;
        gameObject.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;
        playerMovement.enabled = false;

        SetTextPage(JsonUtility.FromJson<DialogPage>(testDialog.text));
    }

    public void Close()
    {
        if (!open) return;

        open = false;
        ClearTextPage();
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
        playerMovement.enabled = true;
        onClose.Invoke();
    }

    private void SetTextPage(DialogPage page)
    {
        ClearTextPage();
        page.options.ForEach(option => AppendText(option));
        typewriterAnimationQueue.RunAtEnd(() => {
            textButtons.ForEach(button => button.enabled = true);
            textOnHovers.ForEach(onHover => onHover.enabled = true);
        });
    }

    private void ClearTextPage()
    {
        textComponents.ForEach(t => Destroy(t));
        textComponents.Clear();
        textButtons.Clear();
        textOnHovers.Clear();
    }

    private void AppendText(DialogOption options)
    {
        typewriterAnimationQueue.AppendCoroutine(AddText(options));
    }

    private IEnumerator AddText(DialogOption option)
    {
        GameObject go = new("TXT", typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(textArea, false);

        rect.pivot = new(option.halign, 1f);
        rect.anchorMin = new(option.halign, 1f);
        rect.anchorMax = new(option.halign, 1f);
        rect.anchoredPosition = Vector2.zero;

        if (textComponents.Count > 0)
        {
            RectTransform lastRect = textComponents.Last().GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(0f, lastRect.anchoredPosition.y - lastRect.sizeDelta.y - verticalSpacing - option.topPadding);
        }

        TextMeshProUGUI textComponent = go.AddComponent<TextMeshProUGUI>();
        textComponent.fontSize = fontSize;
        textComponent.font = font;
        textComponent.color = textColor;

        if (option.clickable)
        {
            Button button = go.AddComponent<Button>();
            button.targetGraphic = textComponent;
            int clickIndex = textButtons.Count;
            button.onClick.AddListener(() => OnTextClicked(clickIndex));
            button.enabled = false;
            textButtons.Add(button);

            OnHover onHover = go.AddComponent<OnHover>();
            onHover.onHoverEnter.AddListener(() => { textComponent.color = textHoverColor; });
            onHover.onHoverExit.AddListener(() => { textComponent.color = textColor; });
            onHover.enabled = false;
            textOnHovers.Add(onHover);
        }

        textComponent.SetText(option.text);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textArea.rect.width);
        textComponent.ForceMeshUpdate();
        rect.sizeDelta = new(textComponent.renderedWidth, textComponent.renderedHeight);

        yield return AnimateTypewriter(textComponent, option.text);
        textComponents.Add(textComponent);
    }

    private IEnumerator AnimateTypewriter(TextMeshProUGUI textComponent, string text)
    {
        StringBuilder sb = new();
        textComponent.SetText("");
        bool insideTag = false;

        foreach (char c in text)
        {
            if (c == '<') insideTag = true;
            sb.Append(c);
            if (c == '>') insideTag = false;

            if (!insideTag && c != '>' && c != '<' && !char.IsWhiteSpace(c))
            {
                textComponent.SetText(sb.ToString());
                yield return new WaitForSeconds(typingSeconds);
            }
        }

        textComponent.SetText(text);
    }

    private void OnTextClicked(int clickIndex)
    {
        // TODO
        Debug.Log($"Clicked text {clickIndex}");
    }
}
