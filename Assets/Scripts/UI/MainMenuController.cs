using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string song = "Main Menu";

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject TutprialPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button TutorialButton;
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

        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        graphicsDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);

        LoadSettings();

        ShowMainMenu();

        SoundtrackManager.Instance.PlayTrack(song);
    }

    public void PlayGame()
    {
        SceneSwitcher.OpenLevelSelect();
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        TutprialPanel.SetActive(false);
        settingsPanel.SetActive(true);
        creditsPanel.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        TutprialPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }
    
    public void ShowTutorial()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        TutprialPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        TutprialPanel.SetActive(false);
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
        PlayerPrefs.Save();
    }

    public void OnGraphicsQualityChanged(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        graphicsDropdown.value = savedQuality;
        
        QualitySettings.SetQualityLevel(savedQuality);
    }
}