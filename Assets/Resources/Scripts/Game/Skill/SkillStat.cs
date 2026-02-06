using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class SkillStat
{
    private static float BaseSlashMult = 1f;
    private static float BaseDeathBlastMult = 1f;
    private static float BaseOrbitalMult = 1f;
    private static float BaseLaserMult = 1f;

    private static Dictionary<UpgradeType, int> SkillLevels = new Dictionary<UpgradeType, int>();

    private static Dictionary<int, float> UpgradedSlash = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedDeathBlast = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedOrbital = new Dictionary<int, float>();
    private static Dictionary<int, float> UpgradedLaser = new Dictionary<int, float>();

    public static float CurSlashMult;
    public static float CurDeathBlastMult;
    public static float CurOrbitalMult;
    public static float CurLaserMult;

    public static void UpdateSkillContribution(UpgradeType type, int uniqueID, float value, int level)
    {
        if(!SkillLevels.ContainsKey(type) || SkillLevels[type] < level) SkillLevels[type] = level;

        switch (type)
        {
            case UpgradeType.SkillSlash: UpgradedSlash[uniqueID] = value; break;
            case UpgradeType.SkillDeathBlast: UpgradedDeathBlast[uniqueID] = value; break;
            case UpgradeType.SkillOrbital: UpgradedOrbital[uniqueID] = value; break;
            case UpgradeType.SkillLaser: UpgradedLaser[uniqueID] = value; break;
        }

        RefreshSkillStats();
    }

    public static void RefreshSkillStats()
    {
        CurSlashMult = BaseSlashMult + UpgradedSlash.Values.Sum();
        CurDeathBlastMult = BaseDeathBlastMult + UpgradedDeathBlast.Values.Sum();
        CurOrbitalMult = BaseOrbitalMult + UpgradedOrbital.Values.Sum();
        CurLaserMult = BaseLaserMult + UpgradedLaser.Values.Sum();

    }

    public static bool IsUnlocked(UpgradeType type)
    {
        return SkillLevels.TryGetValue(type, out var level) && level >= 1;
    }

    public static int GetSkillLevel(UpgradeType type)
    {
        if(SkillLevels.TryGetValue(type, out var level))
        {
            return level;
        }

        return 0;
    }

    public static string GetStatValue(UpgradeType type)
    {
        float value = 0;

        switch (type)
        {
            case UpgradeType.SkillSlash: value = CurSlashMult; break;
            case UpgradeType.SkillDeathBlast: value = CurDeathBlastMult; break;
            case UpgradeType.SkillOrbital: value = CurOrbitalMult; break;
            case UpgradeType.SkillLaser: value = CurLaserMult; break;

            default: return "null";
        }

        return value.ToString("0.##");
    }
}
