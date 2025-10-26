using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private RectTransform textArea;

    [Header("Text Parameters")]
    [SerializeField] private float typeSpeed = 0.05f;
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float fontSize = 24;
    [SerializeField] private TMP_FontAsset font;

    public Action onClose;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerMovement playerMovement;

    private bool open = true;

    private readonly List<TextMeshProUGUI> textComponents = new();
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

        AppendText("Welcome!", true);
        AppendText("More text ...", true);
        AppendText("--> Reply", false, extraVerticalSpacing: 10f);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
        camera.EnableCamera();
        spellManager.enabled = true;
        playerMovement.enabled = true;
        onClose.Invoke();
    }

    private void AppendText(string text, bool left_align, float extraVerticalSpacing = 0f)
    {
        typewriterAnimationQueue.AppendCoroutine(AddText(text, left_align, extraVerticalSpacing));
    }

    private IEnumerator AddText(string text, bool left_align, float extraVerticalSpacing)
    {
        GameObject go = new("TXT", typeof(RectTransform));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(textArea, false);

        rect.pivot = new(0.5f, 1f);
        rect.anchorMin = new(0f, 1f);
        rect.anchorMax = new(1f, 1f);
        rect.offsetMin = new(0f, rect.offsetMin.y);
        rect.offsetMax = new(0f, rect.offsetMax.y);
        rect.anchoredPosition = Vector2.zero;

        if (textComponents.Count > 0)
        {
            RectTransform lastRect = textComponents.Last().GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(0f, lastRect.anchoredPosition.y - lastRect.sizeDelta.y - verticalSpacing - extraVerticalSpacing);
        }

        TextMeshProUGUI textComponent = go.AddComponent<TextMeshProUGUI>();
        textComponent.alignment = left_align ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
        textComponent.fontSize = fontSize;
        textComponent.font = font;
        // TODO color, clickable (add button component)

        textComponent.SetText(text);
        FitTextHeight(textComponent);
        yield return AnimateTypewriter(textComponent, text);
        textComponents.Add(textComponent);
    }

    private void FitTextHeight(TextMeshProUGUI textComponent)
    {
        textComponent.ForceMeshUpdate();
        float height = textComponent.preferredHeight;
        RectTransform rect = textComponent.GetComponent<RectTransform>();
        rect.sizeDelta = new(rect.sizeDelta.x, height);
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
}
