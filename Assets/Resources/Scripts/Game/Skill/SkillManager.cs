using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    /// <summary>
    /// CL
    /// </summary>
    private float _chainLightningTimer = 0f;
    private const float _chainLightningCooldown = 1f;
    private const int _maxChainCount = 10;

    public void Init()
    {
        _chainLightningTimer = 0f;
    }

    public void ProcessSkillsCooldown()
    {
        if (SkillStat.IsUnlocked(UpgradeType.SkillChainLightning))
        {
            _chainLightningTimer += Time.deltaTime;
        }
    }

    #region Lightning
    public void TryChainLightning()
    {
        if (!SkillStat.IsUnlocked(UpgradeType.SkillChainLightning)) return;
        if (_chainLightningTimer < _chainLightningCooldown) return;

        var circle = GameManager.Instance.Circle;
        Vector2 circlePos = circle.GetTransform.position;
        float range = circle.Radius * 3;

        var targets = MonsterManager.Instance.GetMonstersInChainLightningRange(range)
            .OrderBy(x => ((Vector2)x.GetTransform.position - circlePos).sqrMagnitude)
            .Take(_maxChainCount)
            .ToList();

        if(targets.Count > 0)
        {
            CastLightning(targets);
        }

        ResetChainLightningTImer();
    }

    private void CastLightning(List<BaseMonster> targets)
    {
        var (baseDam, isCri) = GameManager.Instance.Circle.GetCalcDamage();
        int skillDamage = Mathf.Max(1, Mathf.RoundToInt(baseDam * SkillStat.CurChainLightningMult));

        foreach(var t in targets)
        {
            if (t == null) continue;

            t.TakeDamage(skillDamage, isCri);
        }
    }

    private void ResetChainLightningTImer()
    {
        _chainLightningTimer = 0f;
    }
    #endregion
}
