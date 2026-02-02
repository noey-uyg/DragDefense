using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public static class CSVParser
{
    private const string UpgradeDataPath = "CSV/UpgradeData";

    public static Dictionary<int, UpgradeData> UpgradeDataDict = new Dictionary<int, UpgradeData>();

    public static void AllCSVLoad()
    {
        LoadUpgradeData();
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
}
