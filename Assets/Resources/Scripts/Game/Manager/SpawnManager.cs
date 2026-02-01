using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Transform _bossSpawnPoint;
    [SerializeField] private Transform _monsterTransform;
    [SerializeField] private List<MonsterData> _datas;

    private float _spawnTime = 0f;
    private float _playTime = 0f;
    private bool _hasBossSpawn;

    private const float _bossAppearTime = 70f;
    private const int _bossAppearLevel = 6;

    private readonly Dictionary<int, int[]> _weightTable = new Dictionary<int, int[]>()
    {
        {0, new int[] {100,0,0,0,0,0,0}},
        {1, new int[] {80,20,0,0,0,0}},
        {2, new int[] {60,30,10,0,0,0,0}},
        {3, new int[] {40,30,20,10,0,0,0}},
        {4, new int[] {20,30,25,15,10,0,0}},
        {5, new int[] {10,20,25,20,15,10,0}},
        {6, new int[] {5,10,15,20,25,15,10}}
        //{0, new int[] {100,0,0,0,0,0,0}},
        //{1, new int[] {0,100,0,0,0,0,0}},
        //{2, new int[] {0,0,100,0,0,0,0}},
        //{3, new int[] {0,0,0,100,0,0,0}},
        //{4, new int[] {0,0,0,0,100,0,0}},
        //{5, new int[] {0,0,0,0,0,100,0}},
        //{6, new int[] {0,0,0,0,0,0,100}}
    };

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
            return;

        _playTime += Time.deltaTime;
        _spawnTime += Time.deltaTime;

        if(!_hasBossSpawn && _playTime >= _bossAppearTime && PlayerStat.CurMonsterLevel >= _bossAppearLevel)
        {
            SpawnBoss();
        }

        if (_spawnTime > PlayerStat.CurSpawnTime)
        {
            _spawnTime = 0f;
            Spawn();
        }
    }

    private void Spawn()
    {
        int spawnCount = Random.Range(1, ((int)PlayerStat.CurMonsterLevel + 1) * 2);

        for(int i = 0; i < spawnCount; i++)
        {
            MonsterData selectData = GetMonsterDataByLevel((int)PlayerStat.CurMonsterLevel);

            BaseMonster monster = MonsterPool.Instance.GetNormalMonster();
            monster.GetTransform.position = _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
            monster.GetTransform.SetParent(_monsterTransform);
            monster.Init(selectData);
        }
    }

    private void SpawnBoss()
    {
        _hasBossSpawn = true;

        MonsterData bossData = _datas.Find(x => x.isBoss);

        if(bossData != null)
        {
            SoundManager.Instance.PlayBGM(BGMType.GameBoss);

            var boss = MonsterPool.Instance.GetBossMonster();
            boss.GetTransform.position = _bossSpawnPoint.position;
            boss.GetTransform.SetParent(_monsterTransform);
            boss.Init(bossData);
        }
    }

    public void ResetBossState()
    {
        _hasBossSpawn = false;
        _playTime = 0;
    }

    private MonsterData GetMonsterDataByLevel(int level)
    {
        int currentLevel = _weightTable.ContainsKey(level) ? level : _weightTable.Keys.Max();
        int[] weights = _weightTable[currentLevel];

        int totalWeight = weights.Sum();
        int randomValue = Random.Range(0, totalWeight);
        int currentSum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            currentSum += weights[i];

            if (randomValue <= currentSum) return _datas[i];
        }

        return _datas[0];
    }
}
