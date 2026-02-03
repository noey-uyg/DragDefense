using System;
using UnityEngine;

public class LocalizationManager : Singleton<LocalizationManager>
{
    private Language _curLanguage = Language.KOR;
    
    public Language CurLanguage { get { return _curLanguage; } }
    public Action OnLanguageChanged;

    public void Init()
    {
        
    }

    public void ChangeLanguage(Language lang)
    {
        _curLanguage = lang;
        OnLanguageChanged?.Invoke();
    }

    public string GetText(string key)
    {
        var data = CSVParser.GetLocalizationData(key);

        if (data == null) return key;

        return _curLanguage switch
        {
            Language.KOR => data.KOR,
            Language.ENG => data.ENG,
            Language.JPN => data.JPN,
            _ => data.ENG
        };
    }

    public string GetFormatText(string key, params object[] args)
    {
        string baseText = GetText(key);
        try
        {
            return string.Format(baseText, args);
        }
        catch (FormatException)
        {
            Debug.LogError($"[Localization] 포맷 오류 : {key}");
            return baseText;
        }
    }

    public void SetDefaultLanguage()
    {
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Korean: _curLanguage = Language.KOR; break;
            case SystemLanguage.Japanese: _curLanguage = Language.JPN;break;
            default: _curLanguage = Language.ENG; break;
        }

        DataManager.SaveLanguageData(_curLanguage);
    }

    public void RegisterLanguageEvent(Action callback)
    {
        OnLanguageChanged += callback;
    }

    public void UnregisterLanguageEvent(Action callback)
    {
        OnLanguageChanged -= callback;
    }
}
