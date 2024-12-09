using System.Collections.Generic;
using UnityEngine;

public static class Settings
{
    public static float masterV = 10;
    public static float sfxV = 10;
    public static float musicV = 10;

    public static int resolutionIndex = 0;
    public static string resolution = "";
    public static int fullscreenMode = 0; //0=exclusive, 1=fullscreen window, 2=window, 3=maximized window

    public static float aimAssist = 10;

    public static string lang = "en";

    public static List<string> resolutionOptions;

    const string MASTER_VOLUME = "SETTING_MASTER_VOLUME";
    const string SFX_VOLUME = "SETTING_SFX_VOLUME";
    const string MUSIC_VOLUME = "SETTING_MUSIC_VOLUME";
    const string RESOLUTION = "SETTING_RESOLUTION";
    const string FULLSCREEN_MODE = "SETTING_FULLSCREEN_MODE";
    const string AIM_ASSIST = "SETTING_AIM_ASSIST";
    const string LANGUAGE = "SETTING_LANGUAGE";

    public static bool CheckSettingSaves()
    {
        if (!Save.HasKey(MASTER_VOLUME)) return false;
        if (!Save.HasKey(SFX_VOLUME)) return false;
        if (!Save.HasKey(MUSIC_VOLUME)) return false;
        if (!Save.HasKey(RESOLUTION)) return false;
        if (!Save.HasKey(FULLSCREEN_MODE)) return false;
        if (!Save.HasKey(AIM_ASSIST)) return false;
        if (!Save.HasKey(LANGUAGE)) return false;
        return true;
    }

    public static void LoadSettings()
    {
        if (!CheckSettingSaves()) SaveInitialSettings();

        masterV = Save.GetFloat(MASTER_VOLUME);
        sfxV = Save.GetFloat(SFX_VOLUME);
        musicV = Save.GetFloat(MUSIC_VOLUME);

        resolutionOptions = GetResolutionOptions();
        resolution = Save.GetString(RESOLUTION);
        resolutionIndex = GetCurrentResolutionIndex();
        fullscreenMode = Save.GetInt(FULLSCREEN_MODE);

        aimAssist = Save.GetFloat(AIM_ASSIST);

        lang = Save.GetString(LANGUAGE);
    }

    public static void SaveSettings()
    {
        Save.SaveFloat(MASTER_VOLUME, masterV);
        Save.SaveFloat(SFX_VOLUME, sfxV);
        Save.SaveFloat(MUSIC_VOLUME, musicV);

        Save.SaveString(RESOLUTION, resolution);
        Save.SaveInt(FULLSCREEN_MODE, fullscreenMode);

        Save.SaveFloat(AIM_ASSIST, aimAssist);

        Save.SaveString(LANGUAGE, lang);
    }

    public static void SaveInitialSettings()
    {
        Save.SaveFloat(MASTER_VOLUME, 10);
        Save.SaveFloat(SFX_VOLUME, 10);
        Save.SaveFloat(MUSIC_VOLUME, 10);

        Resolution res = Screen.resolutions[^1];
        Save.SaveString(RESOLUTION, res.width + "x" + res.height);
        Save.SaveInt(FULLSCREEN_MODE, 0);

        Save.SaveFloat(AIM_ASSIST, 5);

        Save.SaveString(LANGUAGE, Application.systemLanguage==SystemLanguage.Korean?"Korean":"English");
    }

    public static void ApplySettings()
    {
        ApplyResolution(resolution, fullscreenMode);
        Locale.SetLanguage(lang == "Korean" ? Language.Korean : Language.English);
        SoundSystem.SetVolume();
    }

    public static List<string> GetResolutionOptions()
    {
        Resolution[] resolutions = Screen.resolutions;
        List<string> result = new();
        foreach (Resolution res in resolutions)
        {
            string temp = res.width + "x" + res.height;
            if (result.Contains(temp)) continue;
            result.Add(temp);
        }
        return result;
    }

    public static int GetCurrentResolutionIndex()
    {
        if (resolutionOptions == null) resolutionOptions = GetResolutionOptions();
        if (resolutionOptions.Contains(resolution)) return resolutionOptions.FindIndex(0, x => x == resolution);
        return resolutionOptions.Count - 1;
    }

    private static void ApplyResolution(string res,int mode)
    {
        if (!int.TryParse(res.Split('x')[0],out int _)) res = GetResolutionOptions()[^1];
        Screen.SetResolution(int.Parse(res.Split('x')[0]), int.Parse(res.Split('x')[1]),
            mode==0?FullScreenMode.ExclusiveFullScreen:
            mode==1?FullScreenMode.FullScreenWindow:
            mode==3?FullScreenMode.MaximizedWindow:
            FullScreenMode.Windowed);
    }
}
