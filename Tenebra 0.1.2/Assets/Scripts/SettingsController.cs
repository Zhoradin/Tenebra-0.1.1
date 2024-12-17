using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject optionsPanel, closeWithoutSavingPanel, videoPanel, audioPanel, controlsPanel;

    public TMP_Dropdown resolutionDropdown, screenModeDropdown, fpsDropdown;
    public Toggle limitFPSToggle;
    public Button applyButton, closeButton, defaultsButton;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate, tempMasterValue, tempMusicValue, tempSFXValue;
    private float defaultMasterValue = 0.5f;
    private float defaultMusicValue = 0.5f;
    private float defaultSFXValue = 0.5f;
    private int currentResolutionIndex, currentFPSIndex, tempResolutionIndex, tempScreenModeIndex, tempFPSIndex, defaultResolutionIndex, defaultScreenModeIndex, defaultFPSIndex;
    private FullScreenMode currentScreenMode;
    private bool isFPSToggleOn, tempFPSToggleOn;
    private bool defaultFPSToggleOn = false;

    private GameController gameController;

    private List<string> fpsOptions = new List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "Unlimited FPS" };

    public AudioMixer myMixer;
    public Slider masterSlider, musicSlider, SFXSlider;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        optionsPanel.SetActive(false);
        closeWithoutSavingPanel.SetActive(false);

        FormResolutions();
        FormScreenModes();
        FormFPSOptions();

        if (PlayerPrefs.HasKey("musicVolume") || PlayerPrefs.HasKey("masterVolume") || PlayerPrefs.HasKey("sfxVolume") || PlayerPrefs.HasKey("screenModeIndex") || PlayerPrefs.HasKey("resolutionIndex") || PlayerPrefs.HasKey("fpsIndex"))
        {
            LoadSettings();
        }
        else
        {
            ChangeMasterVolume();
            ChangeMusicVolume();
            ChangeSFXVolume();
        }

        

        tempResolutionIndex = currentResolutionIndex;
        tempScreenModeIndex = screenModeDropdown.value;
        tempFPSToggleOn = isFPSToggleOn;
        tempFPSIndex = currentFPSIndex;

        defaultResolutionIndex = currentResolutionIndex;
        defaultScreenModeIndex = 0;
        defaultFPSIndex = 4;

        UpdateDefaultsButtonState();
        ApplySettings();
    }

    public void OnVideoClicked()
    {
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
        AudioManager.instance.PlaySFX(0);
    }

    public void OnAudioClicked()
    {
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
        AudioManager.instance.PlaySFX(0);
    }

    public void OnControlsClicked()
    {
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
        AudioManager.instance.PlaySFX(0);
    }

    public void OnCloseButtonClicked()
    {
        if (applyButton.interactable == true)
        {
            Debug.Log("deneme");
            closeWithoutSavingPanel.SetActive(true);
        }
        else
        {
            Debug.Log("deneme2");
            optionsPanel.SetActive(false);
        }
        AudioManager.instance.PlaySFX(0);
    }

    public void OnDefaultsButtonClicked()
    {
        resolutionDropdown.value = defaultResolutionIndex;
        screenModeDropdown.value = defaultScreenModeIndex;
        limitFPSToggle.isOn = defaultFPSToggleOn;
        fpsDropdown.value = defaultFPSIndex;
        fpsDropdown.interactable = defaultFPSToggleOn;

        masterSlider.value = defaultMasterValue;
        musicSlider.value = defaultMusicValue;
        SFXSlider.value = defaultSFXValue;

        applyButton.interactable = true;
        UpdateDefaultsButtonState();
        AudioManager.instance.PlaySFX(0);
    }

    public void OnKeepChangesButtonClicked()
    {
        ApplySettings();
        closeWithoutSavingPanel.SetActive(false);
        optionsPanel.SetActive(false);
        AudioManager.instance.PlaySFX(0);
    }

    public void OnDiscardChangesButtonClicked()
    {
        DiscardSettings();
        closeWithoutSavingPanel.SetActive(false);
        optionsPanel.SetActive(false);
        AudioManager.instance.PlaySFX(0);
    }

    public void DiscardSettings()
    {
        resolutionDropdown.value = tempResolutionIndex;
        screenModeDropdown.value = tempScreenModeIndex;
        limitFPSToggle.isOn = tempFPSToggleOn;
        fpsDropdown.value = tempFPSIndex;
        fpsDropdown.interactable = tempFPSToggleOn;

        applyButton.interactable = false;
    }

    private void UpdateDefaultsButtonState()
    {
        bool isDefault =
            resolutionDropdown.value == defaultResolutionIndex &&
            screenModeDropdown.value == defaultScreenModeIndex &&
            limitFPSToggle.isOn == defaultFPSToggleOn &&
            fpsDropdown.value == defaultFPSIndex &&
            masterSlider.value == defaultMasterValue &&
            musicSlider.value == defaultMusicValue &&
            SFXSlider.value == defaultSFXValue;

        defaultsButton.interactable = !isDefault;
    }

    private void OnFPSToggleChanged(bool isOn)
    {
        fpsDropdown.interactable = isOn;

        if (isOn)
        {
            if (fpsOptions.Contains("Unlimited FPS"))
            {
                fpsOptions.Remove("Unlimited FPS");
                fpsDropdown.ClearOptions();
                fpsDropdown.AddOptions(fpsOptions);
                fpsDropdown.value = 1;
                fpsDropdown.RefreshShownValue();
            }
        }
        else
        {
            if (!fpsOptions.Contains("Unlimited FPS"))
            {
                fpsOptions.Add("Unlimited FPS");
                fpsDropdown.ClearOptions();
                fpsDropdown.AddOptions(fpsOptions);
                fpsDropdown.value = fpsOptions.Count - 1;
                fpsDropdown.RefreshShownValue();
            }
        }
    }

    public void OnFPSToggleClicked()
    {
        OnFPSToggleChanged(limitFPSToggle.isOn);
        applyButton.interactable = true;
        AudioManager.instance.PlaySFX(0);

        if (tempFPSToggleOn != limitFPSToggle.isOn)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    public void ApplySettings()
    {
        tempResolutionIndex = resolutionDropdown.value;
        tempScreenModeIndex = screenModeDropdown.value;
        tempFPSToggleOn = limitFPSToggle.isOn;
        tempFPSIndex = fpsDropdown.value;

        tempMasterValue = masterSlider.value;
        tempMusicValue = musicSlider.value;
        tempSFXValue = SFXSlider.value;

        applyButton.interactable = false;
        AudioManager.instance.PlaySFX(0);
        UpdateDefaultsButtonState();

        PlayerPrefs.SetInt("screenModeIndex", screenModeDropdown.value);
        PlayerPrefs.SetInt("resolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("fpsIndex", fpsDropdown.value);

        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("sfxVolume", SFXSlider.value);
    }

    private int GetCurrentScreenMode()
    {
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.Windowed:
                return 0;
            case FullScreenMode.ExclusiveFullScreen:
                return 1;
            case FullScreenMode.FullScreenWindow:
                return 2;
            default:
                return 0;
        }
    }

    private int GetFPSValue(string fpsOption)
    {
        switch (fpsOption)
        {
            case "30 FPS": return 30;
            case "60 FPS": return 60;
            case "120 FPS": return 120;
            case "144 FPS": return 144;
            case "Unlimited FPS": return -1;
            default: return -1;
        }
    }

    public void SetResolution()
    {
        ChangeResolution();
        if (tempResolutionIndex != resolutionDropdown.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    private void ChangeResolution()
    {
        Resolution resolution = filteredResolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
        AudioManager.instance.PlaySFX(0);
    }

    public void SetScreenMode(int screenModeIndex)
    {
        ChangeScreenMode(screenModeIndex);
        if (tempScreenModeIndex != screenModeDropdown.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    private void ChangeScreenMode(int screenModeIndex)
    {
        FullScreenMode mode = FullScreenMode.Windowed;
        switch (screenModeIndex)
        {
            case 0: mode = FullScreenMode.Windowed; break;
            case 1: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 2: mode = FullScreenMode.FullScreenWindow; break;
        }
        Screen.fullScreenMode = mode;
        Screen.SetResolution(Screen.width, Screen.height, mode);
        AudioManager.instance.PlaySFX(0);
    }

    public void SetFPSLimit(int fpsIndex)
    {
        ChangeFPSLimit(fpsIndex);
        if (tempFPSIndex != fpsDropdown.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    private void ChangeFPSLimit(int fpsIndex)
    {
        int[] fpsValues = { 30, 60, 120, 144, -1 };
        Application.targetFrameRate = (fpsIndex >= 0 && fpsIndex < fpsValues.Length) ? fpsValues[fpsIndex] : -1;
        AudioManager.instance.PlaySFX(0);
    }

    public void SetMasterVolume()
    {
        ChangeMasterVolume();

        if (tempMasterValue != masterSlider.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    private void ChangeMasterVolume()
    {
        float volume = masterSlider.value;
        myMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetMusicVolume()
    {
        ChangeMusicVolume();

        if (tempMusicValue != musicSlider.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    public void ChangeMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void SetSFXVolume()
    {
        ChangeSFXVolume();

        if (tempSFXValue != SFXSlider.value)
        {
            applyButton.interactable = true;
        }
        else
        {
            applyButton.interactable = false;
        }
    }

    public void ChangeSFXVolume()
    {
        myMixer.SetFloat("sfx", Mathf.Log10(SFXSlider.value) * 20);
        
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
            tempMasterValue = masterSlider.value;
            myMixer.SetFloat("master", Mathf.Log10(masterSlider.value) * 20);
        }
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            tempMusicValue = musicSlider.value;
            myMixer.SetFloat("music", Mathf.Log10(musicSlider.value) * 20);
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            SFXSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            tempSFXValue = SFXSlider.value;
            myMixer.SetFloat("sfx", Mathf.Log10(SFXSlider.value) * 20);
        }
        if (PlayerPrefs.HasKey("screenModeIndex"))
        {
            screenModeDropdown.value = PlayerPrefs.GetInt("screenModeIndex");
            tempScreenModeIndex = screenModeDropdown.value;
        }
        if (PlayerPrefs.HasKey("resolutionIndex"))
        {
            resolutionDropdown.value = PlayerPrefs.GetInt("resolutionIndex");            
            tempResolutionIndex = resolutionDropdown.value;
        }
        if (PlayerPrefs.HasKey("fpsIndex"))
        {
            fpsDropdown.value = PlayerPrefs.GetInt("fpsIndex");
            if (fpsDropdown.value != 4)
            {
                limitFPSToggle.isOn = true;
            }
            tempFPSIndex = fpsDropdown.value;
        }
    }

    private void FormResolutions()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if ((float)resolutions[i].refreshRateRatio.value == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        filteredResolutions.Sort((a, b) =>
        {
            if (a.width != b.width)
                return b.width.CompareTo(a.width);
            else
                return b.height.CompareTo(a.height);
        });

        List<Resolution> mostUsedResolutions = new List<Resolution>();
        mostUsedResolutions.Add(filteredResolutions[0]);
        mostUsedResolutions.Add(filteredResolutions[1]);
        mostUsedResolutions.Add(filteredResolutions[filteredResolutions.Count / 2]);
        mostUsedResolutions.Add(filteredResolutions[filteredResolutions.Count - 1]);

        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < mostUsedResolutions.Count; i++)
        {
            string resolutionOption = mostUsedResolutions[i].width + "x" + mostUsedResolutions[i].height + " " + mostUsedResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz";
            resolutionOptions.Add(resolutionOption);

            if (mostUsedResolutions[i].width == Screen.width && mostUsedResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(resolutionOptions);
        
        resolutionDropdown.RefreshShownValue();
    }

    private void FormScreenModes()
    {
        List<string> screenModeOptions = new List<string> { "Windowed", "Fullscreen", "Borderless" };
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(screenModeOptions);
        currentScreenMode = Screen.fullScreenMode;
        screenModeDropdown.value = GetCurrentScreenMode();
        screenModeDropdown.RefreshShownValue();
    }

    private void FormFPSOptions()
    {
        fpsDropdown.ClearOptions();
        fpsDropdown.AddOptions(fpsOptions);
        fpsDropdown.value = fpsOptions.Count - 1;
        fpsDropdown.RefreshShownValue();
        fpsDropdown.interactable = false;
    }
}
