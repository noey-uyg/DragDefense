using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class CSVParser
{
    private const string UpgradeDataPath = "CSV/UpgradeData";
    private const string LocalizationDataPath = "CSV/LocalizationData";

    public static Dictionary<int, UpgradeData> UpgradeDataDict = new Dictionary<int, UpgradeData>();
    public static Dictionary<string, LocalizationData> LocalizeDict = new Dictionary<string, LocalizationData>();

    public static void AllCSVLoad()
    {
        LoadUpgradeData();
        LoadLocalizationData();
    }

    private static string LoadCSV(string path)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);
        if(asset == null)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없음 {path}");
            return string.Empty;
        }

        return asset.text;
    }

    #region 업그레이드 데이터
    private static void LoadUpgradeData()
    {
        var csv = LoadCSV(UpgradeDataPath);

        if (string.IsNullOrEmpty(csv)) return;

        UpgradeDataDict.Clear();

        string[] lines = csv.Split('\n');

        for(int i=1;i<lines.Length; i++)
        {
            string line = lines[i].Trim();

            if(string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(",");

            UpgradeData data = new UpgradeData();

            data.Type = (UpgradeType)int.Parse(values[0]);
            data.TypeInt = int.Parse(values[0]);
            data.ID = int.Parse(values[1]);
            data.Name = values[2];
            data.Description = values[3];

            data.MaxLevel = int.Parse(values[4]);

            data.cost = new string[data.MaxLevel + 1]; // values[5]부터 [9]까지
            for(int x = 0; x < data.MaxLevel; x++)
            {
                data.cost[x+1] = values[5 + x];
            }

            data.Value = new float[data.MaxLevel + 1]; // values[10]부터 [14]까지
            for (int x = 0; x < data.MaxLevel; x++)
            {
                data.Value[x+1] = float.Parse(values[10 + x]);
            }

            data.GridX = float.Parse(values[15]);
            data.GridY = float.Parse(values[16]);
            data.connectID = int.Parse(values[17]);
            data.connectMax = int.Parse(values[18]) > 0;

            UpgradeDataDict.Add(data.ID, data);
        }
    }
    #endregion

    #region 로컬라이징 데이터
    private static void LoadLocalizationData()
    {
        var csv = LoadCSV(LocalizationDataPath);

        if (string.IsNullOrEmpty(csv)) return;
        
        LocalizeDict.Clear();

        string[] lines = csv.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if(string.IsNullOrEmpty(line)) continue;

            string[] values = line.Split(",");

            LocalizationData data = new LocalizationData
            {
                Key = values[0].Trim(),
                KOR = values[1].Trim().Replace("\\n", "\n"),
                ENG = values[2].Trim().Replace("\\n", "\n"),
                JPN = values[3].Trim().Replace("\\n", "\n"),
            };

            if (!LocalizeDict.ContainsKey(data.Key))
            {
                LocalizeDict.Add(data.Key, data);
            }
        }
    }

    public static LocalizationData GetLocalizationData(string key)
    {
        if(!LocalizeDict.TryGetValue(key, out var data))
        {
            Debug.LogError($"[Localization] 키를 찾을 수 없음 : {key}");
            return null;
        }

        return data;
    }
    #endregion
}
