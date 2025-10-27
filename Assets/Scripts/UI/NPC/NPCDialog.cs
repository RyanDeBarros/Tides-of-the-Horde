using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DynamicTextOptions
{
    public string text;
    public bool left_align = true;
    public bool clickable = false;
    public float extraVerticalSpacing = 0f;
}

public class DynamicTextPage
{
    public float extraVerticalSpacing = 0f;

    private readonly List<DynamicTextOptions> options = new();
    private bool currentlyNPC = true;

    public DynamicTextPage(float extraVerticalSpacing)
    {
        this.extraVerticalSpacing = extraVerticalSpacing;
    }

    public IReadOnlyList<DynamicTextOptions> GetOptions()
    {
        return options.AsReadOnly();
    }

    public void AddNPCText(string text)
    {
        options.Add(new() { text = text, left_align = true, clickable = false, extraVerticalSpacing = currentlyNPC ? 0f : extraVerticalSpacing });
        currentlyNPC = true;
    }

    public void AddPlayerClickableText(string text)
    {
        options.Add(new() { text = text, left_align = false, clickable = true, extraVerticalSpacing = currentlyNPC ? extraVerticalSpacing : 0f });
        currentlyNPC = false;
    }
}

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private RectTransform textArea;

    [Header("Text Parameters")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float speakerSwitchSpacing = 10f;
    [SerializeField] private float fontSize = 24;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color textHoverColor = Color.blue;
    [SerializeField] private TMP_FontAsset font;

    public Action onClose;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerMovement playerMovement;

    private bool open = true;

    private readonly List<TextMeshProUGUI> textComponents = new();
    private int numberOfClickableTexts = 0;
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
        open = true;
        gameObject.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;
        playerMovement.enabled = false;

        DynamicTextPage page = CreatePage();
        page.AddNPCText("Welcome");
        page.AddNPCText("More text ...");
        page.AddPlayerClickableText("--> Reply");
        SetTextPage(page);
    }

    public void Close()
    {
        open = false;
        ClearTextPage();
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
        playerMovement.enabled = true;
        onClose.Invoke();
    }

    private DynamicTextPage CreatePage()
    {
        return new(speakerSwitchSpacing);
    }

    private void SetTextPage(DynamicTextPage page)
    {
        ClearTextPage();
        foreach (var option in page.GetOptions())
            AppendText(option);
    }

    private void ClearTextPage()
    {
        textComponents.ForEach(t => Destroy(t));
        textComponents.Clear();
        numberOfClickableTexts = 0;
    }

    private void AppendText(DynamicTextOptions options)
    {
        typewriterAnimationQueue.AppendCoroutine(AddText(options));
    }

    private IEnumerator AddText(DynamicTextOptions options)
    {
        GameObject go = new("TXT", typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(textArea, false);

        rect.pivot = new(options.left_align ? 0f : 1f, 1f);
        rect.anchorMin = new(options.left_align ? 0f : 1f, 1f);
        rect.anchorMax = new(options.left_align ? 0f : 1f, 1f);
        rect.anchoredPosition = Vector2.zero;

        if (textComponents.Count > 0)
        {
            RectTransform lastRect = textComponents.Last().GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(0f, lastRect.anchoredPosition.y - lastRect.sizeDelta.y - verticalSpacing - options.extraVerticalSpacing);
        }

        TextMeshProUGUI textComponent = go.AddComponent<TextMeshProUGUI>();
        textComponent.alignment = options.left_align ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
        textComponent.fontSize = fontSize;
        textComponent.font = font;
        textComponent.color = textColor;

        if (options.clickable)
        {
            Button button = go.AddComponent<Button>();
            button.targetGraphic = textComponent;
            int clickIndex = numberOfClickableTexts;
            ++numberOfClickableTexts;
            button.onClick.AddListener(() => OnTextClicked(clickIndex));

            OnHover onHover = go.AddComponent<OnHover>();
            onHover.onHoverEnter.AddListener(() => { textComponent.color = textHoverColor; });
            onHover.onHoverExit.AddListener(() => { textComponent.color = textColor; });
        }

        textComponent.SetText(options.text);
        textComponent.ForceMeshUpdate();
        rect.sizeDelta = new(textComponent.preferredWidth, textComponent.preferredHeight);

        yield return AnimateTypewriter(textComponent, options.text);
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
                yield return new WaitForSeconds(typeSpeed);
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
