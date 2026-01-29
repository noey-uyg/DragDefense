using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // [CL]
    private float _chainLightningTimer = 0f;
    private const float _chainLightningCooldown = 1f;
    private const int _maxChainCount = 10;
    // [Blast]
    private List<BlastShot> _activeBlast = new List<BlastShot>();
    public List<BlastShot> GetActiveBlasts() => _activeBlast;
    private int _blastAttackCount = 0;
    private const int _blastTriggetCount = 5;

    public void Init()
    {
        _chainLightningTimer = 0f;
        _blastAttackCount = 0;
    }

    public void ProcessSkillsCooldown()
    {
        if (SkillStat.IsUnlocked(UpgradeType.SkillChainLightning))
        {
            _chainLightningTimer += Time.deltaTime;
        }
    }

    public void CircleAttackCheck()
    {
        TryChainLightning();
        TryBlast();
    }

    #region Lightning
    /// <summary>
    /// 기본 공격 시 발동
    /// </summary>
    public void TryChainLightning()
    {
        if (!SkillStat.IsUnlocked(UpgradeType.SkillChainLightning) || _chainLightningTimer < _chainLightningCooldown) return;

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

    #region BlastShot
    /// <summary>
    /// 기본 공격 시 발동
    /// </summary>

    public void TryBlast()
    {
        //if (!SkillStat.IsUnlocked(UpgradeType.SkillDeathBlast)) return;

        _blastAttackCount++;

        if(_blastAttackCount >= _blastTriggetCount)
        {
            _blastAttackCount = 0;
            CastBlast();
        }
    }

    private void CastBlast()
    {
        var circle = GameManager.Instance.Circle;
        Vector2 startPos = circle.GetTransform.position;
        var (baseDam, isCri) = circle.GetCalcDamage();
        int skillDamage = Mathf.Max(1, Mathf.RoundToInt(baseDam * SkillStat.CurDeathBlastMult));

        for(int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            var blast = SkillPool.Instance.GetBlastShot();
            blast.Init(startPos, dir, skillDamage, isCri);
            _activeBlast.Add(blast);
        }
    }

    public void ProcessActiveBlastMove(float dt)
    {
        for (int i = _activeBlast.Count - 1; i >= 0; i--)
        {
            _activeBlast[i].Move(dt);
            Vector3 pos = _activeBlast[i].GetTransform.position;
            
            if(Mathf.Abs(pos.x) > 10 || Mathf.Abs(pos.y) > 7)
            {
                SkillPool.Instance.ReleaseBlastShot(_activeBlast[i]);
                _activeBlast.RemoveAt(i);
            }
        }
    }

    #endregion
}
