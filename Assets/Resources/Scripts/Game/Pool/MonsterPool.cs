using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class MonsterPool : Singleton<MonsterPool>
{
    [SerializeField] private BaseMonster _normalMonsterPrefab;    
    [SerializeField] private BaseMonster _bossMonsterPrefab;

    private ObjectPool<BaseMonster> _normalMonsterPool;
    private ObjectPool<BaseMonster> _bossMonsterPool;

    private const int MAXSIZE = 1000;
    private const int INITSIZE = 20;

    protected override void OnAwake()
    {
        _normalMonsterPool = new ObjectPool<BaseMonster>(CreateNormalMonster, ActiavateNormalMonster, DisableNormalMonster, DestroyNormalMonster, false, INITSIZE, MAXSIZE);
        _bossMonsterPool = new ObjectPool<BaseMonster>(CreateBossMonster, ActiavateBossMonster, DisableBossMonster, DestroyBossMonster, false, INITSIZE, MAXSIZE);
    }

    #region 일반 몬스터
    private BaseMonster CreateNormalMonster()
    {
        return Instantiate(_normalMonsterPrefab);
    }

    private void ActiavateNormalMonster(BaseMonster monster)
    {
        monster.gameObject.SetActive(true);
    }
    private void DisableNormalMonster(BaseMonster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void DestroyNormalMonster(BaseMonster monster)
    {
        Destroy(monster);
    }

    public BaseMonster GetNormalMonster()
    {
        BaseMonster monster = null;
        if(_normalMonsterPool.CountActive >= MAXSIZE)
        {
            monster = CreateNormalMonster();
        }
        else
        {
            monster = _normalMonsterPool.Get();
        }

        return monster;
    }

    public void ReleaseNormalMonster(BaseMonster monster)
    {
        _normalMonsterPool.Release(monster);
    }
    #endregion

    #region 보스 몬스터
    private BaseMonster CreateBossMonster()
    {
        return Instantiate(_bossMonsterPrefab);
    }

    private void ActiavateBossMonster(BaseMonster monster)
    {
        monster.gameObject.SetActive(true);
    }
    private void DisableBossMonster(BaseMonster monster)
    {
        monster.gameObject.SetActive(false);
    }

    private void DestroyBossMonster(BaseMonster monster)
    {
        Destroy(monster);
    }

    public BaseMonster GetBossMonster()
    {
        BaseMonster monster = null;
        if (_bossMonsterPool.CountActive >= MAXSIZE)
        {
            monster = CreateBossMonster();
        }
        else
        {
            monster = _bossMonsterPool.Get();
        }

        return monster;
    }

    public void ReleaseBossMonster(BaseMonster monster)
    {
        _bossMonsterPool.Release(monster);
    }
    #endregion
}
