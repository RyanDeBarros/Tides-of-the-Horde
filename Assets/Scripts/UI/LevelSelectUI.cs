using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectUI : MonoBehaviour
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

    private const string HIGHEST_UNLOCKED_LEVEL_INDEX = "HIGHEST_UNLOCKED_LEVEL_INDEX";

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
        int highestUnlocked = PlayerDataManager.GetInt(HIGHEST_UNLOCKED_LEVEL_INDEX, 0);

        for (int i = 0; i < levels.Count; i++)
        {
            bool unlocked = (i <= highestUnlocked);

            if (levels[i].dim) levels[i].dim.SetActive(!unlocked);
            if (levels[i].lockIcon) levels[i].lockIcon.SetActive(!unlocked);
            if (levels[i].button) levels[i].button.interactable = unlocked;

            int captured = i;
            levels[i].button.onClick.RemoveAllListeners();
            levels[i].button.onClick.AddListener(() => {
                if (captured <= PlayerDataManager.GetInt(HIGHEST_UNLOCKED_LEVEL_INDEX, 0))
                    SceneManager.LoadScene(levels[captured].sceneName);
                else
                    Debug.Log($"Level {captured + 1} is locked.");
            });
        }
    }

    private static void MarkLevelCompleted(int indexJustCleared)
    {
        int highest = PlayerDataManager.GetInt(HIGHEST_UNLOCKED_LEVEL_INDEX, 0);
        int next = indexJustCleared + 1;
        if (next > highest)
            PlayerDataManager.SetInt(HIGHEST_UNLOCKED_LEVEL_INDEX, next);
    }

    public static void CompleteLevel(int levelIndex)
    {
        MarkLevelCompleted(levelIndex);
        SceneManager.LoadScene("LevelSelectScene");
    }
}
