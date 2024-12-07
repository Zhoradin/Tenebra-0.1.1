using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public static SettingsController instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject optionsPanel;
    [SerializeField] private GameObject closeWithoutSavingPanel;

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown screenModeDropdown;
    public Toggle limitFPSToggle;
    public TMP_Dropdown fpsDropdown;
    public Button applyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button defaultsButton;

    public GameObject videoPanel, audioPanel, controlsPanel;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex;
    private FullScreenMode currentScreenMode;
    private bool isFPSToggleOn;
    private int currentFPSIndex;

    // De�i�iklikleri saklamak i�in
    private int tempResolutionIndex;
    private int tempScreenModeIndex;
    private bool tempFPSToggleOn;
    private int tempFPSIndex;

    // Default De�erler
    private int defaultResolutionIndex;
    private int defaultScreenModeIndex;
    private bool defaultFPSToggleOn;
    private int defaultFPSIndex;

    private GameController gameController;

    private List<string> fpsOptions = new List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "Unlimited FPS" };

    void Start()
    {
        gameController = FindObjectOfType<GameController>();

        optionsPanel.SetActive(false);
        closeWithoutSavingPanel.SetActive(false);

        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();
        currentRefreshRate = (float)Screen.currentResolution.refreshRateRatio.value;

        // ��z�n�rl�kleri filtrele ve s�rala
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

        // ��z�n�rl�kleri ekle
        List<string> resolutionOptions = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRateRatio.value.ToString("0.##") + " Hz";
            resolutionOptions.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Screen mode se�eneklerini ekle
        List<string> screenModeOptions = new List<string> { "Windowed", "Fullscreen", "Borderless" };
        screenModeDropdown.ClearOptions();
        screenModeDropdown.AddOptions(screenModeOptions);
        currentScreenMode = Screen.fullScreenMode;
        screenModeDropdown.value = GetCurrentScreenMode();
        screenModeDropdown.RefreshShownValue();

        // FPS se�eneklerini ekle
        fpsDropdown.ClearOptions();
        fpsDropdown.AddOptions(fpsOptions);
        fpsDropdown.value = fpsOptions.Count - 1; // Default Unlimited FPS
        fpsDropdown.RefreshShownValue();
        fpsDropdown.interactable = false; // Ba�lang��ta kapal�

        // Ba�lang�� ayarlar�n� kaydet
        isFPSToggleOn = limitFPSToggle.isOn;
        currentFPSIndex = fpsDropdown.value;
        tempResolutionIndex = currentResolutionIndex;
        tempScreenModeIndex = screenModeDropdown.value;
        tempFPSToggleOn = isFPSToggleOn;
        tempFPSIndex = currentFPSIndex;

        // Apply buton ba�lang��ta kapal�
        applyButton.interactable = false; // <-- This line ensures Apply button is initially disabled

        // Eventler
        resolutionDropdown.onValueChanged.AddListener(OnSettingsChanged);
        screenModeDropdown.onValueChanged.AddListener(OnSettingsChanged);
        limitFPSToggle.onValueChanged.AddListener(OnFPSToggleChanged);
        fpsDropdown.onValueChanged.AddListener(OnSettingsChanged);

        defaultResolutionIndex = currentResolutionIndex;
        defaultScreenModeIndex = screenModeDropdown.value;
        defaultFPSToggleOn = isFPSToggleOn;
        defaultFPSIndex = currentFPSIndex;

        UpdateDefaultsButtonState();
    }

    private void OnSettingsChanged(int value)
    {
        applyButton.interactable = true;
        UpdateDefaultsButtonState();
    }

    public void OnVideoClicked()
    {
        videoPanel.SetActive(true);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void OnAudioClicked()
    {
        videoPanel.SetActive(false);
        audioPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }

    public void OnControlsClicked()
    {
        videoPanel.SetActive(false);
        audioPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    private void UpdateDefaultsButtonState()
    {
        bool isDefault =
            resolutionDropdown.value == defaultResolutionIndex &&
            screenModeDropdown.value == defaultScreenModeIndex &&
            limitFPSToggle.isOn == defaultFPSToggleOn &&
            fpsDropdown.value == defaultFPSIndex;

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
                fpsDropdown.value = 1; // Varsay�lan olarak 60 FPS
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
                fpsDropdown.value = fpsOptions.Count - 1; // Varsay�lan olarak Unlimited FPS
                fpsDropdown.RefreshShownValue();
            }
        }

        UpdateDefaultsButtonState();
    }

    public void OnFPSToggleClicked()
    {
        OnFPSToggleChanged(true);
        applyButton.interactable = true;
    }

    public void OnCloseButtonClicked()
    {
        // De�i�iklik yap�ld� m� veya yap�lan de�i�iklikler eski haline getirildi mi kontrol et
        bool settingsChanged =
            resolutionDropdown.value != tempResolutionIndex ||
            screenModeDropdown.value != tempScreenModeIndex ||
            limitFPSToggle.isOn != tempFPSToggleOn ||
            fpsDropdown.value != tempFPSIndex;

        // E�er ayarlarda de�i�iklik yap�lm��sa, 'closeWithoutSavingPanel'� a�
        if (settingsChanged)
        {
            closeWithoutSavingPanel.SetActive(true);
        }
        else
        {
            optionsPanel.SetActive(false);
        }
    }


    public void OnKeepChangesButtonClicked()
    {
        ApplySettings();
        closeWithoutSavingPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OnDiscardChangesButtonClicked()
    {
        CloseSettings();
        closeWithoutSavingPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }

    public void OnDefaultsButtonClicked()
    {
        // Varsay�lan de�erlere d�n
        resolutionDropdown.value = defaultResolutionIndex;
        screenModeDropdown.value = defaultScreenModeIndex;
        limitFPSToggle.isOn = defaultFPSToggleOn;
        fpsDropdown.value = defaultFPSIndex;
        fpsDropdown.interactable = defaultFPSToggleOn;

        // Apply ve Defaults butonlar�n�n durumunu g�ncelle
        applyButton.interactable = true;
        UpdateDefaultsButtonState();
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

    public void ApplySettings()
    {
        // Ayarlar� uygula
        currentResolutionIndex = resolutionDropdown.value;
        currentScreenMode = (FullScreenMode)screenModeDropdown.value;
        isFPSToggleOn = limitFPSToggle.isOn;
        currentFPSIndex = fpsDropdown.value;

        SetResolution(currentResolutionIndex);
        SetScreenMode(screenModeDropdown.value);
        SetFPSLimit(isFPSToggleOn ? currentFPSIndex : -1);

        // FPS de�erini almak i�in fpsOptions listesini kullan�yoruz
        int fpsValue = (currentFPSIndex >= 0 && currentFPSIndex < fpsOptions.Count)
            ? GetFPSValue(fpsOptions[currentFPSIndex])
            : -1;

        // E�er FPS dropdown aktifse, FPS de�erini kaydet
        if (fpsDropdown.interactable)
        {
            DataCarrier.instance.FPSValue = currentFPSIndex; // Kaydedilen de�er: FPS Dropdown'dan se�ilen index
            DataCarrier.instance.FPSIndex = fpsValue; // Kaydedilen de�er: FPS Dropdown'dan se�ilen de�erin ger�ek FPS kar��l��� (30, 60, 120 vb.)
        }

        // Uygulanan ayarlar� ge�ici ayarlara kaydet
        tempResolutionIndex = currentResolutionIndex;
        tempScreenModeIndex = screenModeDropdown.value;
        tempFPSToggleOn = isFPSToggleOn;
        tempFPSIndex = currentFPSIndex;

        DataCarrier.instance.FPSToggleOn = isFPSToggleOn;

        applyButton.interactable = false;
        gameController.SaveGame();
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

    public void CloseSettings()
    {
        resolutionDropdown.value = tempResolutionIndex;
        screenModeDropdown.value = tempScreenModeIndex;
        limitFPSToggle.isOn = tempFPSToggleOn;
        fpsDropdown.value = tempFPSIndex;
        fpsDropdown.interactable = tempFPSToggleOn;

        applyButton.interactable = false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
    }

    public void SetScreenMode(int screenModeIndex)
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
    }

    public void SetFPSLimit(int fpsIndex)
    {
        int[] fpsValues = { 30, 60, 120, 144, -1 }; // -1 = unlimited
        Application.targetFrameRate = (fpsIndex >= 0 && fpsIndex < fpsValues.Length) ? fpsValues[fpsIndex] : -1;
    }
}
