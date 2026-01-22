using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public static class PlayerStat
{
    // [초기 수치]
    // [Game]
    private const float BasePlayTime = 10f;
    private const float BaseGoldGainPercent = 1f;
    private const float BaseSpawnTime = 1.5f;
    private const float BaseMonsterLevel = 0;
    // [Center]
    private const float BaseHP = 20f;
    private const float BaseDamageReduction = 0f;
    // [Circle]
    private const float BaseAtk = 1f;
    private const float BaseAtkDelay = 0.15f;
    private const float BaseRadius = 0.25f;
    private const float BaseCritical = 0f;

    // [현재 수치]
    // [Game]
    public static float CurPlayTime;
    public static float CurGoldGainPercent;
    public static float CurSpawnTime;
    public static float CurMonsterLevel;
    private static int _curGold;
    public static int CurGold {get => _curGold; set { _curGold = value; OnGoldChanged?.Invoke(_curGold); } }
    public static System.Action<int> OnGoldChanged;
    // [Center]
    public static float CurMaxHP;
    public static float CurDamageReduction;
    // [Circle]
    public static float CurAtk;
    public static float CurAtkDelay;
    public static float CurRadius;
    public static float CurCritical;

    // [누적 수치]
    // [Game]
    private static Dictionary<int, float> UpgradedPlayTime = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedGoldGainPercent = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedSpawnTime = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedMonsterLevel = new Dictionary<int, float>();
    // [Center]
    private static Dictionary<int, float> UpgradedHP = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedDefense = new Dictionary<int, float>();
    // [Circle]
    private static Dictionary<int, float> UpgradedAtk = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedAtkDelay = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedRadius = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedCritical = new Dictionary<int, float>();

    public static void UpdateContiribution(UpgradeType type, int uniqueID, float value)
    {
        switch (type)
        {
            case UpgradeType.GamePlayTime: UpgradedPlayTime[uniqueID] = value; break;
            case UpgradeType.GameGoldGainPercent: UpgradedGoldGainPercent[uniqueID] = value; break;
            case UpgradeType.GameSpawnTime: UpgradedSpawnTime[uniqueID] = value; break;
            case UpgradeType.CenterHP: UpgradedHP[uniqueID] = value; break;
            case UpgradeType.CenterDefense: UpgradedDefense[uniqueID] = value; break;
            case UpgradeType.CircleAtk: UpgradedAtk[uniqueID] = value; break;
            case UpgradeType.CircleAtkDelay: UpgradedAtkDelay[uniqueID] = value; break;
            case UpgradeType.CircleRadius: UpgradedRadius[uniqueID] = value; break;
            case UpgradeType.CircleCiritical: UpgradedCritical[uniqueID] = value; break;
        }
        RefreshStats();
    }

    public static void RefreshStats()
    {
        CurPlayTime = BasePlayTime + UpgradedPlayTime.Values.Sum();
        CurGoldGainPercent = BaseGoldGainPercent + UpgradedGoldGainPercent.Values.Sum();
        CurSpawnTime = BaseSpawnTime + UpgradedSpawnTime.Values.Sum();
        CurMonsterLevel = BaseMonsterLevel + UpgradedMonsterLevel.Values.Sum();

        CurMaxHP = BaseHP + UpgradedHP.Values.Sum();
        CurDamageReduction = BaseDamageReduction + UpgradedDefense.Values.Sum();

        CurAtk = BaseAtk + UpgradedAtk.Values.Sum();
        CurAtkDelay = BaseAtkDelay + UpgradedAtkDelay.Values.Sum();
        CurRadius = BaseRadius + UpgradedRadius.Values.Sum();
        CurCritical = BaseCritical + UpgradedCritical.Values.Sum();
    }
}
