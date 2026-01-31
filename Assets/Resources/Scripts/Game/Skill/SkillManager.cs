using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : Singleton<SkillManager>
{
    // [CL]
    private float _chainSlashTimer = 0f;
    private float _chainSlashCooldown;
    private const int _maxChainCount = 10;
    // [Blast]
    private List<BlastShot> _activeBlast = new List<BlastShot>();
    public List<BlastShot> GetActiveBlasts() => _activeBlast;
    
    private int _blastAttackCount = 0;
    private const int _blastTriggetCount = 5;
    // [Orbital]
    private List<Orbital> _activeOrbitals = new List<Orbital>();
    public List<Orbital> GetActiveOrbitals() => _activeOrbitals;

    private float _orbitalSpeed;
    private int _orbitalCount;
    private float _orbitalAngle = 0f;
    private float _baseRadius;
    private const float _baseRotateSpeed = 90f;    

    public void Init()
    {
        InitSlash();
        _blastAttackCount = 0;
        SyncOrbitalCount();
    }

    public void CleanUp()
    {
        ClearBlast();
        ClearOrbitals();
    }

    public void SkillProcess(float dt)
    {
        ProcessSkillsCooldown();
        ProcessActiveBlastMove(dt);
        ProcessOrbitalRotation(dt);
    }

    public void ProcessSkillsCooldown()
    {
        if (SkillStat.IsUnlocked(UpgradeType.SkillSlash))
        {
            _chainSlashTimer += Time.deltaTime;
        }
    }

    public void CircleAttackCheck()
    {
        TrySlash();
        TryBlast();
    }

    #region Slash
    /// <summary>
    /// 기본 공격 시 발동
    /// </summary>
    
    private void InitSlash()
    {
        _chainSlashTimer = 0f;
        _chainSlashCooldown = PlayerStat.CurAtkDelay;
    }

    public void TrySlash()
    {
        if (!SkillStat.IsUnlocked(UpgradeType.SkillSlash) || _chainSlashTimer < _chainSlashCooldown) return;

        var circle = GameManager.Instance.Circle;
        float range = circle.Radius * 3;
        Vector2 circlePos = circle.GetTransform.position;
        Vector3 effectScale = new Vector3(range, range, 1);

        var targets = MonsterManager.Instance.GetMonstersInSlashRange(range)
            .OrderBy(x => ((Vector2)x.GetTransform.position - circlePos).sqrMagnitude)
            .Take(_maxChainCount)
            .ToList();

        if(targets.Count > 0)
        {
            CastSlash(targets, circlePos, effectScale);
        }

        ResetChainSlashTImer();
    }

    private void CastSlash(List<BaseMonster> targets, Vector2 circlePos, Vector3 effectScale)
    {
        EffectManager.PlayEffect(EffectType.SlashEffect, circlePos, Quaternion.identity, effectScale);

        var (baseDam, isCri) = GameManager.Instance.Circle.GetCalcDamage();
        int skillDamage = Mathf.Max(1, Mathf.RoundToInt(baseDam * SkillStat.CurSlashMult));

        foreach(var t in targets)
        {
            if (t == null) continue;

            t.TakeDamage(skillDamage, isCri);
        }
    }

    private void ResetChainSlashTImer()
    {
        _chainSlashTimer = 0f;
    }
    #endregion

    #region BlastShot
    /// <summary>
    /// 기본 공격 시 발동
    /// </summary>

    public void TryBlast()
    {
        if (!SkillStat.IsUnlocked(UpgradeType.SkillDeathBlast)) return;

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

    private void ClearBlast()
    {
        for (int i = _activeBlast.Count - 1; i >= 0; i--)
        {
            if (_activeBlast[i] != null)
            {
                SkillPool.Instance.ReleaseBlastShot(_activeBlast[i]);
            }
        }

        _activeBlast.Clear();
    }

    #endregion

    #region Orbital
    public void ProcessOrbitalRotation(float dt)
    {
        if(!SkillStat.IsUnlocked(UpgradeType.SkillOrbital) || _activeOrbitals.Count == 0) return;

        _orbitalAngle += _baseRotateSpeed * _orbitalSpeed * dt;

        for(int i = 0; i < _activeOrbitals.Count; i++)
        {
            _activeOrbitals[i].SetPositionByAngle(_orbitalAngle, _baseRadius);
        }
    }

    private void SyncOrbitalCount()
    {
        if (!SkillStat.IsUnlocked(UpgradeType.SkillOrbital)) return;

        _baseRadius = PlayerStat.CurRadius + 1;
        _orbitalCount = SkillStat.GetSkillLevel(UpgradeType.SkillOrbital) + 1;
        _orbitalSpeed = PlayerStat.CalcCurAtkDelay;
        _orbitalAngle = 0;

        ClearOrbitals();

        for(int i=0;i< _orbitalCount; i++)
        {
            var orbital = SkillPool.Instance.GetOrbital();

            float startAngle = (360 / _orbitalCount) * i;

            Vector2 startDir = new Vector2(
                Mathf.Cos(startAngle * Mathf.Deg2Rad),
                Mathf.Sin(startAngle * Mathf.Deg2Rad)
                );

            orbital.SetDirection(startDir);
            orbital.SetPositionByAngle(_orbitalAngle, _baseRadius);

            _activeOrbitals.Add(orbital);
        }
    }

    private void ClearOrbitals()
    {
        for (int i = _activeOrbitals.Count - 1; i >= 0; i--)
        {
            SkillPool.Instance.ReleaseOrbital(_activeOrbitals[i]);
        }

        _activeOrbitals.Clear();
    }

    #endregion
}
