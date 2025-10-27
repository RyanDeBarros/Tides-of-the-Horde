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
    public string clickResponse;
    public float topPadding = 0f;
    public float halign = 0f;
    public bool challenge = false;
    public bool reward = false;
}

[SerializeField]
public class DialogPage
{
    public List<DialogOption> options;
}

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private RectTransform textArea;
    [SerializeField] private TextAsset dialogFile;

    [Header("Text Parameters")]
    [SerializeField] private float typingSeconds = 0.02f;
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float fontSize = 16;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color textHoverColor = Color.blue;
    [SerializeField] private TMP_FontAsset font;

    public Action onClose;

    private new PlayerCamera camera;
    private SpellManager spellManager;
    private PlayerMovement playerMovement;

    private ChallengeTracker challengeTracker;

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
        challengeTracker = FindObjectsByType<ChallengeTracker>(FindObjectsSortMode.None).GetUniqueElement();

        typewriterAnimationQueue = gameObject.AddComponent<CoroutineQueue>();

        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (open) return;

        open = true;
        gameObject.SetActive(true);
        camera.DisableCamera();
        spellManager.enabled = false;
        playerMovement.enabled = false;

        challengeTracker.SelectRandomChallenge();
        SetTextPage(JsonUtility.FromJson<DialogPage>(dialogFile.text));
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
        // TODO while typewriting - play typing SFX: begin continuously playing here, and then stop it in RunAtEnd().
        page.options.ForEach(option => typewriterAnimationQueue.AppendCoroutine(AddText(option)));
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
            MakeTextClickable(textComponent, option.clickResponse);

        string text = option.text;
        if (option.challenge)
            text = challengeTracker.GetChallengeStatement() ?? text;
        if (option.reward)
            text = challengeTracker.GetRewardStatement() ?? text;
        
        textComponent.SetText(text);
        textComponent.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textArea.rect.width);
        textComponent.ForceMeshUpdate();
        rect.sizeDelta = new(textComponent.renderedWidth, textComponent.renderedHeight);

        yield return AnimateTypewriter(textComponent, text);
        textComponents.Add(textComponent);
    }

    private void MakeTextClickable(TextMeshProUGUI textComponent, string clickResponse)
    {
        Button button = textComponent.gameObject.AddComponent<Button>();
        button.targetGraphic = textComponent;
        button.onClick.AddListener(() => OnTextClicked(clickResponse));
        button.enabled = false;
        textButtons.Add(button);

        OnHover onHover = textComponent.gameObject.AddComponent<OnHover>();
        onHover.onHoverEnter.AddListener(() => { textComponent.color = textHoverColor; });
        onHover.onHoverExit.AddListener(() => { textComponent.color = textColor; });
        onHover.enabled = false;
        textOnHovers.Add(onHover);
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

    private void OnTextClicked(string clickResponse)
    {
        Dictionary<string, Action> actions = new(StringComparer.InvariantCultureIgnoreCase) {
            ["accept"] = challengeTracker.AcceptChallenge,
            ["decline"] = challengeTracker.DeclineChallenge,
        };

        if (actions.TryGetValue(clickResponse, out Action action))
            action();
        else
            Debug.LogError($"Unrecognized click response: {clickResponse}");

        Close();
    }
}
