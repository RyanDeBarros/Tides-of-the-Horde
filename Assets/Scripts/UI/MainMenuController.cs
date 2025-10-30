using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Settings References")]
    public Slider volumeSlider;
    public TMPro.TMP_Dropdown graphicsDropdown;
    public Button settingsBackButton;

    [Header("Credits References")]
    public Button creditsBackButton;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        LoadSettings();

        ShowMainMenu();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("purple-red-Arena"); 
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnVolumeChanged(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    public void OnGraphicsQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
    }

    void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        graphicsDropdown.value = savedQuality;
        QualitySettings.SetQualityLevel(savedQuality);
    }
}