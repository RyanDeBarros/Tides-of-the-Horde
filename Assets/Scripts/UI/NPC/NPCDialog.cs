using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

enum ClickResponse
{
    Accept,
    Decline,
    Claim,
    Continue
}

[Serializable]
class DialogOption
{
    public string text;
    public bool clickable = false;
    public ClickResponse clickResponse = ClickResponse.Continue;
    public float topPadding = 0f;
    public bool challenge = false;
    public bool reward = false;
}

[SerializeField]
class DialogPage
{
    public List<DialogOption> options;
}

public class NPCDialog : MonoBehaviour
{
    [SerializeField] private RectTransform textArea;
    [SerializeField] private TextAsset openingDialogFile;
    [SerializeField] private TextAsset closingDialogFile;

    [Header("Audio")]
    [SerializeField] private AudioSource typingSFX;
    [SerializeField] private AudioClip acceptAudioClip;
    [SerializeField] private AudioClip declineAudioClip;
    [SerializeField] private AudioClip claimAudioClip;
    [SerializeField] private AudioClip continueAudioClip;

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
        Assert.IsNotNull(typingSFX);
        Assert.IsNotNull(acceptAudioClip);
        Assert.IsNotNull(declineAudioClip);
        Assert.IsNotNull(claimAudioClip);
        Assert.IsNotNull(continueAudioClip);

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
            SetOpeningTextPage(challengeTracker.SelectRandomChallenge());
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
            options.Add(new DialogOption() { challenge = true });
            options.Add(new DialogOption() { reward = true });
            options.Add(new DialogOption() { text = "Accept", topPadding = 10f, clickable = true, clickResponse = ClickResponse.Accept });
            options.Add(new DialogOption() { text = "Decline", clickable = true, clickResponse = ClickResponse.Decline });
        }
        else
        {
            options.Add(new DialogOption() { text = "Unfortunately, I have no more rewards to give for this level...", topPadding = 10f });
            options.Add(new DialogOption() { text = "Continue", topPadding = 10f, clickable = true, clickResponse = ClickResponse.Continue });
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
                options.Add(new DialogOption() { reward = true });
                options.Add(new DialogOption() { text = "Claim Reward", topPadding = 10f, clickable = true, clickResponse = ClickResponse.Claim });
            }
            else
            {
                options.Add(new DialogOption() { text = "Unfortunately, you failed my challenge!", topPadding = 10f });
                options.Add(new DialogOption() { text = "Continue", topPadding = 10f, clickable = true, clickResponse = ClickResponse.Continue });
            }
        }
        else
        {
            options.Add(new DialogOption() { text = "Continue", topPadding = 10f, clickable = true, clickResponse = ClickResponse.Continue });
        }

        SetTextPage(options);
    }

    private void SetTextPage(List<DialogOption> options)
    {
        ClearTextPage();
        typingSFX.Play();

        options.ForEach(option => typewriterAnimationQueue.AppendCoroutine(AddText(option)));
        typewriterAnimationQueue.RunAtEnd(() => {
            textButtons.ForEach(button => button.enabled = true);
            textOnHovers.ForEach(onHover => onHover.enabled = true);
            StartCoroutine(StopTypingSFX());
        });
    }

    private IEnumerator StopTypingSFX()
    {
        yield return new WaitForSeconds(typingSFX.clip.length - typingSFX.time);
        typingSFX.Stop();
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

        float halign = option.clickable ? 1f : option.challenge || option.reward ? 0.5f : 0f;
        rect.pivot = new(halign, 1f);
        rect.anchorMin = new(halign, 1f);
        rect.anchorMax = new(halign, 1f);
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

    private void MakeTextClickable(TextMeshProUGUI textComponent, ClickResponse clickResponse)
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

    private void OnTextClicked(ClickResponse clickResponse)
    {
        switch (clickResponse)
        {
            case ClickResponse.Accept:
                SoundEffectPlayer.Play(acceptAudioClip);
                challengeTracker.AcceptChallenge();
                break;
            case ClickResponse.Decline:
                SoundEffectPlayer.Play(declineAudioClip);
                challengeTracker.DeclineChallenge();
                break;
            case ClickResponse.Claim:
                SoundEffectPlayer.Play(claimAudioClip);
                challengeTracker.GiveReward();
                break;
            case ClickResponse.Continue:
                SoundEffectPlayer.Play(continueAudioClip);
                break;
            default:
                Debug.LogError($"Unrecognized click response: {clickResponse}");
                break;
        }

        Close();
    }
}
