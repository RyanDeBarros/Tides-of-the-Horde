using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public static class SceneSwitcher
{
    private static readonly Dictionary<int, string> levelScenes = new() {
        { 0, "Level1" },
        { 1, "Level2" },
        { 2, "Level3" }
    };

    public static void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public static void OpenMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelectScene");
    }

    public static void OpenLevelStatistics()
    {
        SceneManager.LoadScene("LevelStatistics");
    }

    public static void OpenLevel(int levelIndex)
    {
        if (!levelScenes.ContainsKey(levelIndex))
            throw new NotImplementedException($"Level index {levelIndex} is not associated with a scene.");

        SceneManager.LoadScene(levelScenes[levelIndex]);
    }
}
