using TMPro;
using UnityEngine;

public class PauseMenuChallengeUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI challengeText;
    [SerializeField] private TextMeshProUGUI rewardText;

    private ChallengeTracker challengeTracker;

    private void Awake()
    {
        
        challengeTracker = GlobalFind.FindUniqueObjectByType<ChallengeTracker>(true);
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (challengeTracker != null && challengeTracker.HasChallenge())
        {
            string challenge = challengeTracker.GetChallengeStatement();
            string reward = challengeTracker.GetRewardStatement();

            challengeText.text = "<b>Current Challenge:</b>\n" + challenge;
            rewardText.text = "<b>Reward:</b>\n" + reward;
        }
        else
        {
            challengeText.text = "No active challenge";
            rewardText.text = "";
        }
    }
}
