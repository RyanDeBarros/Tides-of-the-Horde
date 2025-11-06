using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelectU : MonoBehaviour
{
    [Serializable]
    public class LevelItem
    {
        public string sceneName;
        public Button button;
        public GameObject dim;
        public GameObject lockIcon;
    }

    public List<LevelItem> levels = new();

    private const string HIGHEST_UNLOCKED_INDEX = "HIGHEST_UNLOCKED_INDEX";

    void Awake()
    {
        InitUI();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void InitUI()
    {
        int highestUnlocked = PlayerPrefs.GetInt(HIGHEST_UNLOCKED_INDEX, 0);

        for (int i = 0; i < levels.Count; i++)
        {
            bool unlocked = (i <= highestUnlocked);

            if (levels[i].dim) levels[i].dim.SetActive(!unlocked);
            if (levels[i].lockIcon) levels[i].lockIcon.SetActive(!unlocked);
            if (levels[i].button) levels[i].button.interactable = unlocked;

            int captured = i;
            levels[i].button.onClick.RemoveAllListeners();
            levels[i].button.onClick.AddListener(() => {
                if (captured <= PlayerPrefs.GetInt(HIGHEST_UNLOCKED_INDEX, 0))
                    SceneManager.LoadScene(levels[captured].sceneName);
                else
                    Debug.Log($"Level {captured + 1} is locked.");
            });
        }
    }

    private static void MarkLevelCompleted(int indexJustCleared)
    {
        int highest = PlayerPrefs.GetInt(HIGHEST_UNLOCKED_INDEX, 0);
        int next = indexJustCleared + 1;
        if (next > highest)
        {
            PlayerPrefs.SetInt(HIGHEST_UNLOCKED_INDEX, next);
            PlayerPrefs.Save();
        }
    }

    // TODO Call LevelSelectUI.CompleteLevel(levelIndex) when exiting portal
    public static void CompleteLevel(int levelIndex)
    {
        MarkLevelCompleted(levelIndex);
        SceneManager.LoadScene("LevelSelectScene");
    }
}
