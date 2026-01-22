using System.Collections.Generic;
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
        PlayerPrefs.SetInt(GoldSaveKey, PlayerStat.CurGold);
        PlayerPrefs.Save();
    }

    public static void LoadGoldData()
    {
        PlayerStat.CurGold = PlayerPrefs.GetInt(GoldSaveKey, 0);
    }
    #endregion

    public static void ResetAll()
    {
        PlayerPrefs.DeleteKey(UpgradeSaveKey);
        PlayerPrefs.DeleteKey(GoldSaveKey);
        PlayerPrefs.Save();
    }
}
