using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class NodeSaveData
{
    public int id;
    public int level;
}

[System.Serializable]
public class UpgradeSaveData
{
    public List<NodeSaveData> datas = new List<NodeSaveData>();
}

public static class DataManager
{
    private const string UpgradeSaveKey = "Upgrade_Save_Key";
    private const string GoldSaveKey = "Gold_Save_Key";
    private const string BGMSaveKey = "BGM_Save_Key";
    private const string SFXSaveKey = "SFX_Save_Key";
    private const string LangSaveKey = "Lang_Save_Key";

    #region 업그레이드 데이터
    public static void SaveUpgradeData(UpgradeSaveData upgradeData)
    {
        string json = JsonUtility.ToJson(upgradeData);
        PlayerPrefs.SetString(UpgradeSaveKey, json);
        PlayerPrefs.Save();
    }

    public static UpgradeSaveData LoadUpgradeData()
    {
        if (!PlayerPrefs.HasKey(UpgradeSaveKey)) return null;

        string json = PlayerPrefs.GetString(UpgradeSaveKey);
        return JsonUtility.FromJson<UpgradeSaveData>(json);
    }
    #endregion

    #region 골드 데이터
    public static void SaveGoldData()
    {
        PlayerPrefs.SetString(GoldSaveKey, PlayerStat.CurGold.ToString());
        PlayerPrefs.Save();
    }

    public static void LoadGoldData()
    {
        PlayerStat.CurGold = BigInteger.Parse(PlayerPrefs.GetString(GoldSaveKey, "0"));
    }
    #endregion

    #region 사운드 데이터
    public static void SaveSoundData(float bgm, float sfx)
    {
        PlayerPrefs.SetFloat(BGMSaveKey, bgm);
        PlayerPrefs.SetFloat(SFXSaveKey, sfx);
        PlayerPrefs.Save();
    }

    public static (float bgm, float sfx) LoadSoundData()
    {
        float bgm = PlayerPrefs.GetFloat(BGMSaveKey, 0.5f);
        float sfx = PlayerPrefs.GetFloat(SFXSaveKey, 0.5f);

        return (bgm, sfx);
    }
    #endregion

    #region 언어 데이터
    public static void SaveLanguageData(Language lang)
    {
        PlayerPrefs.SetInt(LangSaveKey, (int)lang);
        PlayerPrefs.Save();
    }

    public static Language LoadLanguageData()
    {
        int defaultLang = (int)Language.ENG;

        int langIndex = PlayerPrefs.GetInt(LangSaveKey, defaultLang);

        return (Language)langIndex;
    }

    #endregion

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(UpgradeSaveKey);
        PlayerPrefs.DeleteKey(GoldSaveKey);
        PlayerPrefs.DeleteKey(BGMSaveKey);
        PlayerPrefs.DeleteKey(SFXSaveKey);
        PlayerPrefs.DeleteKey(LangSaveKey);
        PlayerPrefs.Save();
    }
}
