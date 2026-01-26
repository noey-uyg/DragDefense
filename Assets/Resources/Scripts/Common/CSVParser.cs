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
            data.GridX = float.Parse(values[9]);
            data.GridY = float.Parse(values[10]);
            data.cost = new string[data.MaxLevel + 1];
            data.Value = new float[data.MaxLevel + 1];
            data.connectID = int.Parse(values[11]);
            data.connectMax = int.Parse(values[12]) > 0;

            BigInteger baseCost = BigInteger.Parse(values[5]);
            float costMult = float.Parse(values[6]);
            float baseValue = float.Parse(values[7]);
            float valueSum = float.Parse(values[8]);

            for (int x = 1; x <= data.MaxLevel; x++)
            {
                data.Value[x] = baseValue + ((x - 1) * valueSum);
                if (x == 1)
                {
                    data.cost[x] = baseCost.ToString();
                }
                else
                {
                    double mult = Mathf.Pow(costMult, x - 1);
                    BigInteger calcCost = new BigInteger((double)baseCost * mult);
                    data.cost[x] = calcCost.ToString();
                }
            }

            UpgradeDataDict.Add(data.ID, data);
        }
    }
}
