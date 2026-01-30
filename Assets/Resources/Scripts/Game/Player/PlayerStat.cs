using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEditor;
using UnityEngine.Rendering;

public static class PlayerStat
{
    // [초기 수치]
    // [Game]
    private const float BasePlayTime = 20f; // 게임 지속 시간 //최대 120
    private const float BaseGoldGainPercent = 1f; // 골드 획득량 배율 //최대 (10 = 1000%)
    private const float BaseSpawnTime = 1.5f; // 몬스터 스폰 시간 감소 //최대 0.25
    private const float BaseMonsterLevel = 0; // 등장 몬스터 레벨 증가 //최대 6
    private const float BaseGoldBonusChance = 0f; // 처치 시 추가 골드 획득 확률 //최대 50
    // [Center]
    private const float BaseHP = 20f; // 센터 체력 //최대 5000+a
    private const float BaseDamageReduction = 0f; // 센터 방어력(적 데미지 - 방어력 = 최종 데미지) //최대 50
    // [Circle]
    private const float BaseAtk = 1f; // 공격력 //최대 5000+a
    private const float BaseAtkDelay = 1.5f; // 공격 속도(감소될수록 빨라짐) //최대 0.2
    private const float BaseRadius = 0.25f; // 공격 반경 증가 //최대 1.5
    private const float BaseCritical = 0f; // 크리티컬 확률 //최대 100
    private const float BaseCriticalDam = 1.5f; // 크리티컬 데미지 배율 //최대 (10 = 1000%)
    private const float BaseVampire = 0f; // 피흡 확률 (1씩 피흡) //최대 10

    // [현재 수치]
    // [Game]
    public static float CurPlayTime;
    public static float CurGoldGainPercent;
    public static float CurSpawnTime;
    public static float CurMonsterLevel;
    private static BigInteger _curGold;
    public static BigInteger CurGold {get => _curGold; set { _curGold = value; OnGoldChanged?.Invoke(_curGold); } }
    public static System.Action<BigInteger> OnGoldChanged;
    public static string CurGoldString { get => CurrencyFomatter.FormatBigInt(CurGold); }
    public static float CurGoldBonusChance;
    // [Center]
    public static float CurMaxHP;
    public static float CurDamageReduction;
    // [Circle]
    public static float CurAtk;
    public static float CurAtkDelay;
    public static float CalcCurAtkDelay { get => BaseAtkDelay / CurAtkDelay; }
    public static float CurRadius;
    public static float CurCritical;
    public static float CurCriticalDam;
    public static float CurVampire;

    // [누적 수치]
    // [Game]
    private static Dictionary<int, float> UpgradedPlayTime = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedGoldGainPercent = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedSpawnTime = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedMonsterLevel = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedGoldBonusChance = new Dictionary<int, float>();
    // [Center]
    private static Dictionary<int, float> UpgradedHP = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedDefense = new Dictionary<int, float>();
    // [Circle]
    private static Dictionary<int, float> UpgradedAtk = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedAtkDelay = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedRadius = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedCritical = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedCiriticalDam = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedVampire = new Dictionary<int, float>();

    public static void UpdateContiribution(UpgradeType type, int uniqueID, float value)
    {
        switch (type)
        {
            case UpgradeType.GamePlayTime: UpgradedPlayTime[uniqueID] = value; break;
            case UpgradeType.GameGoldGainPercent: UpgradedGoldGainPercent[uniqueID] = value; break;
            case UpgradeType.GameSpawnTime: UpgradedSpawnTime[uniqueID] = value; break;
            case UpgradeType.GameMonsterLevel: UpgradedMonsterLevel[uniqueID] = value; break;
            case UpgradeType.GameGoldBonusChance: UpgradedGoldBonusChance[uniqueID] = value; break;

            case UpgradeType.CenterHP: UpgradedHP[uniqueID] = value; break;
            case UpgradeType.CenterDefense: UpgradedDefense[uniqueID] = value; break;

            case UpgradeType.CircleAtk: UpgradedAtk[uniqueID] = value; break;
            case UpgradeType.CircleAtkDelay: UpgradedAtkDelay[uniqueID] = value; break;
            case UpgradeType.CircleRadius: UpgradedRadius[uniqueID] = value; break;
            case UpgradeType.CircleCritical: UpgradedCritical[uniqueID] = value; break;
            case UpgradeType.CircleCriticalDam: UpgradedCiriticalDam[uniqueID] = value; break;
            case UpgradeType.CircleVampire: UpgradedVampire[uniqueID] = value; break;
        }
        RefreshStats();
    }

    public static void RefreshStats()
    {
        CurPlayTime = BasePlayTime + UpgradedPlayTime.Values.Sum();
        CurGoldGainPercent = BaseGoldGainPercent + UpgradedGoldGainPercent.Values.Sum();
        CurSpawnTime = BaseSpawnTime + UpgradedSpawnTime.Values.Sum();
        CurMonsterLevel = BaseMonsterLevel + UpgradedMonsterLevel.Values.Sum();
        CurGoldBonusChance = BaseGoldBonusChance + UpgradedGoldBonusChance.Values.Sum();

        CurMaxHP = BaseHP + UpgradedHP.Values.Sum();
        CurDamageReduction = BaseDamageReduction + UpgradedDefense.Values.Sum();
        //_curGold = 9999999999999999;

        CurAtk = BaseAtk + UpgradedAtk.Values.Sum();
        CurAtkDelay = BaseAtkDelay + UpgradedAtkDelay.Values.Sum();
        CurRadius = BaseRadius + UpgradedRadius.Values.Sum();
        CurCritical = BaseCritical + UpgradedCritical.Values.Sum();
        CurCriticalDam = BaseCriticalDam + UpgradedCiriticalDam.Values.Sum();
        CurVampire = BaseVampire + UpgradedVampire.Values.Sum();
    }
}
