using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelSelectU : MonoBehaviour
{
    [System.Serializable]
    public class LevelItem
    {
        public string sceneName;
        public Button button;
        public GameObject dim;
        public GameObject lockIcon;
    }

    [Header("LevelCore£¨Order£ºLevel I, II, III ...£©")]
    public List<LevelItem> levels = new List<LevelItem>();

    const string KEY = "HIGHEST_UNLOCKED_INDEX";

    void Awake()
    {
        InitUI();
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadValley()
    {
        SceneManager.LoadScene("Ryan's Sandbox");
    }

    void InitUI()
    {

        int highestUnlocked = PlayerPrefs.GetInt(KEY, 0);

        for (int i = 0; i < levels.Count; i++)
        {
            bool unlocked = (i <= highestUnlocked);


            if (levels[i].dim) levels[i].dim.SetActive(!unlocked);
            if (levels[i].lockIcon) levels[i].lockIcon.SetActive(!unlocked);
            if (levels[i].button) levels[i].button.interactable = unlocked;


            int captured = i;
            levels[i].button.onClick.RemoveAllListeners();
            levels[i].button.onClick.AddListener(() =>
            {
                if (captured <= PlayerPrefs.GetInt(KEY, 0))
                {

                    SceneManager.LoadScene(levels[captured].sceneName);
                }
                else
                {

                    Debug.Log($"Level {captured + 1} is locked.");
                }
            });
        }
    }

    public static void MarkLevelCompleted(int indexJustCleared)
    {
        int highest = PlayerPrefs.GetInt(KEY, 0);
        int next = indexJustCleared + 1;
        if (next > highest)
        {
            PlayerPrefs.SetInt(KEY, next);
            PlayerPrefs.Save();
        }
    }

    // Comments for the Scene update
    // In the portal control scripts, we need to add following 2 lines at the end to unlock the second chapter and come back to the level selection scene

    // LevelSelectUI.MarkLevelCompleted(0); // accordingly, after mark complete, the following scene will be unlocked
    // SceneManager.LoadScene("LevelSelectScene");


}
