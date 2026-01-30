using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>
{
    private List<BaseMonster> _monsters = new List<BaseMonster>();
    [SerializeField] private Circle _circle;

    private Vector2 _targetPosition = Vector2.zero;

    public void Register(BaseMonster monster) => _monsters.Add(monster);
    public void Unregister(BaseMonster monster) => _monsters.Remove(monster);

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing || _monsters.Count == 0)
            return;

        float dt = Time.deltaTime;
        Vector2 circlePos = _circle.GetTransform.position;
        bool canAttack = _circle.IsReady();
        float circleRadius = _circle.Radius;

        SkillManager.Instance.SkillProcess(dt);

        if (canAttack)
        {
            _circle.PlayAttackMotion();
            SkillManager.Instance.CircleAttackCheck();
        }

        var activeBlasts = SkillManager.Instance.GetActiveBlasts();
        var activeOrbitals = SkillManager.Instance.GetActiveOrbitals();

        for (int i = _monsters.Count - 1; i >= 0; i--)
        {
            if(i>=_monsters.Count || _monsters[i] == null) continue;

            _monsters[i].UpdateOrbitalTimer(dt);

            // 捞悼
            _monsters[i].MoveToTarget(_targetPosition, dt);

            // 单固瘤 贸府
            if (canAttack)
            {
                if (IsMonsterInRange(_monsters[i], circlePos, circleRadius))
                {
                    var (finalDam, isCritical) = _circle.GetCalcDamage();
                    _monsters[i].TakeDamage(finalDam, isCritical);
                }
            }

            // BlastShot 贸府
            for(int j = 0; j < activeBlasts.Count; j++)
            {
                var blast = activeBlasts[j];
                int mId = _monsters[i].GetInstanceID();

                if (blast.HasHit(mId)) continue;

                if (IsMonsterInRange(_monsters[i], blast.GetTransform.position, blast.Radius))
                {
                    _monsters[i].TakeDamage(blast.Damage, blast.IsCritical);
                    blast.AddHit(mId);
                }
            }

            // Orbital 贸府
            if (_monsters[i].CanHitByOrbital())
            {
                for (int j = 0; j < activeOrbitals.Count; j++)
                {
                    var orbital = activeOrbitals[j];

                    if (IsMonsterInRange(_monsters[i], orbital.GetTranform.position, orbital.Radius))
                    {
                        var (baseDam, iscri) = _circle.GetCalcDamage();

                        int skillDam = Mathf.Max(1, Mathf.RoundToInt(baseDam * SkillStat.CurOrbitalMult));
                        _monsters[i].TakeDamage(skillDam, iscri);

                        _monsters[i].ResetOrbitalHitTimer();
                    }
                }
            }
        }

        if(canAttack) _circle.ResetTimer();
    }

    public void ClearAllMonsters()
    {
        for(int i = _monsters.Count - 1; i >= 0; i--)
        {
            if (_monsters[i] != null)
            {
                _monsters[i].Die(false);
            }
        }

        _monsters.Clear();
    }

    public List<BaseMonster> GetMonstersInChainLightningRange(float range)
    {
        List<BaseMonster> result = new List<BaseMonster>();
        Vector2 circlePos = _circle.GetTransform.position;

        for(int i=0;i< _monsters.Count; i++)
        {
            if (_monsters[i] == null) continue;

            if (IsMonsterInRange(_monsters[i], circlePos, range))
            {
                result.Add(_monsters[i]);
            }
        }

        return result;
    }

    private bool IsMonsterInRange(BaseMonster monster, Vector2 centerPos, float range)
    {
        if(monster == null) return false;

        float sqrDist = (centerPos - (Vector2)monster.GetTransform.position).sqrMagnitude;
        float checkRadius = range + monster.VisualRadius;
        float sqrCheckRadius = checkRadius * checkRadius;

        return sqrDist <= sqrCheckRadius;
    }
}
