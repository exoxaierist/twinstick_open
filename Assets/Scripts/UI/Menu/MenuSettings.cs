using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public GameObject firstSelect;

    public Stepper languageStepper;
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider musicSlider;
    public Stepper displayResStepper;
    public Stepper windowModeStepper;
    public Slider shakeStrengthSlider;

    private int _langIndex;
    private float _masterV;
    private float _sfxV;
    private float _musicV;
    private int _resIndex;
    private int _fullscreenMode;
    private float _shakeStrength;

    [ContextMenu("On Open")]
    public void OnOpen()
    {
        Settings.LoadSettings();

        if(languageStepper!=null) languageStepper.value = Settings.lang == "Korean" ? 1 : 0;
        masterSlider.value = Settings.masterV;
        sfxSlider.value = Settings.sfxV;
        musicSlider.value = Settings.musicV;
        displayResStepper.options = Settings.resolutionOptions;
        displayResStepper.value = Settings.resolutionIndex;
        windowModeStepper.value = Settings.fullscreenMode;
        shakeStrengthSlider.value = Settings.aimAssist;

        EventSystem.current.SetSelectedGameObject(firstSelect);
    }

    public void ApplySettings()
    {
        Settings.masterV = _masterV;
        Settings.sfxV = _sfxV;
        Settings.musicV = _musicV;
        Settings.resolution = displayResStepper.options[_resIndex];
        Settings.fullscreenMode = _fullscreenMode;
        Settings.aimAssist = _shakeStrength;
        if(languageStepper!=null) Settings.lang = _langIndex == 1 ? "Korean" : "English";
        Settings.SaveSettings();
        Settings.ApplySettings();
    }

    public void OnLangChange(int value)
    {
        _langIndex = value;
    }

    public void OnMasterChange(float value)
    {
        _masterV = value;
    }
    public void OnSfxChange(float value)
    {
        _sfxV = value;
    }
    public void OnMusicChange(float value)
    {
        _musicV = value;
    }
    public void OnResChange(int value)
    {
        _resIndex = value;
    }
    public void OnWindowChange(int value)
    {
        _fullscreenMode = value;
    }
    public void OnShakeChange(float value)
    {
        _shakeStrength = value;
    }
}
