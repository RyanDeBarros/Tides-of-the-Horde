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
    [SerializeField] private TextAsset openingDialogFile;
    [SerializeField] private TextAsset closingDialogFile;

    [Header("Text Parameters")]
    [SerializeField] private float typingSeconds = 0.01f;
    [SerializeField] private float verticalSpacing = 10f;
    [SerializeField] private float fontSize = 16;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color textHoverColor = Color.blue;
    [SerializeField] private TMP_FontAsset font;

    public Action onClose;

    private PlayerEnabler player;

    private ChallengeTracker challengeTracker;

    private bool open = false;

    private readonly List<TextMeshProUGUI> textComponents = new();
    private readonly List<Button> textButtons = new();
    private readonly List<OnHover> textOnHovers = new();
    private CoroutineQueue typewriterAnimationQueue;

    public enum DialogPhase
    {
        Opening,
        Closing
    }

    public DialogPhase dialogPhase = DialogPhase.Opening;

    public void Initialize(TextAsset openingDialogFile, TextAsset closingDialogFile)
    {
        this.openingDialogFile = openingDialogFile;
        this.closingDialogFile = closingDialogFile;
    }

    private void Awake()
    {
        Assert.IsNotNull(textArea);
        Assert.IsNotNull(font);

        player = GlobalFind.FindUniqueObjectByType<PlayerEnabler>(true);
        challengeTracker = GlobalFind.FindUniqueObjectByType<ChallengeTracker>(true);

        typewriterAnimationQueue = gameObject.AddComponent<CoroutineQueue>();

        gameObject.SetActive(false);
    }

    private void Start()
    {
        Assert.IsNotNull(openingDialogFile);
        Assert.IsNotNull(closingDialogFile);
    }

    public void Open()
    {
        if (open) return;

        open = true;
        gameObject.SetActive(true);
        player.DisablePlayer();

        if (dialogPhase == DialogPhase.Opening)
            SetOpeningTextPage(challengeTracker.SelectRandomChallenge(GetLevelIndex()));
        else if (dialogPhase == DialogPhase.Closing)
            SetClosingTextPage();
        else
            throw new NotImplementedException();
    }

    public void Close()
    {
        if (!open) return;

        open = false;
        ClearTextPage();
        gameObject.SetActive(false);
        player.EnablePlayer();
        onClose.Invoke();
    }

    private void SetOpeningTextPage(bool includeChallenge)
    {
        List<DialogOption> options = JsonUtility.FromJson<DialogPage>(openingDialogFile.text).options;
        if (includeChallenge)
        {
            options.Add(new DialogOption() { text = "Complete this challenge and I'll reward you:", topPadding = 10f });
            options.Add(new DialogOption() { challenge = true, halign = 0.5f });
            options.Add(new DialogOption() { reward = true, halign = 0.5f });
            options.Add(new DialogOption() { text = "Accept", topPadding = 10f, halign = 1f, clickable = true, clickResponse = "accept" });
            options.Add(new DialogOption() { text = "Decline", halign = 1f, clickable = true, clickResponse = "decline" });
        }
        else
        {
            options.Add(new DialogOption() { text = "Unfortunately, I have no more rewards to give for this level...", topPadding = 10f });
            options.Add(new DialogOption() { text = "Continue", topPadding = 10f, halign = 1f, clickable = true, clickResponse = "decline" });
        }

        SetTextPage(options);
    }

    private void SetClosingTextPage()
    {
        List<DialogOption> options = JsonUtility.FromJson<DialogPage>(closingDialogFile.text).options;

        if (challengeTracker.HasChallenge())
        {
            if (challengeTracker.ChallengeCompleted())
            {
                options.Add(new DialogOption() { text = "You completed my challenge! Here is your reward:", topPadding = 10f });
                options.Add(new DialogOption() { reward = true, halign = 0.5f });
            }
            else
                options.Add(new DialogOption() { text = "Unfortunately, you failed my challenge!", topPadding = 10f });
        }

        options.Add(new DialogOption() { text = "Continue", topPadding = 10f, halign = 1f, clickable = true, clickResponse = "claim" });
        SetTextPage(options);
    }

    private void SetTextPage(List<DialogOption> options)
    {
        ClearTextPage();
        // TODO while typewriting - play typing SFX: begin continuously playing here, and then stop it in RunAtEnd().
        options.ForEach(option => typewriterAnimationQueue.AppendCoroutine(AddText(option)));
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
            ["claim"] = () => challengeTracker.RewardIfSuccess(GetLevelIndex())
        };

        if (actions.TryGetValue(clickResponse, out Action action))
            action();
        else
            Debug.LogError($"Unrecognized click response: {clickResponse}");

        Close();
    }

    public int GetLevelIndex()
    {
        return GlobalFind.FindUniqueObjectByType<Portal>(true).levelIndex;
    }
}
