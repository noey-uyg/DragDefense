using UnityEngine;

public class BossMonster : BaseMonster
{
    private Vector2 _spawnPosition;

    public override void Init(MonsterData data)
    {
        base.Init(data);
        _spawnPosition = _transform.position;
    }

    protected override void AttackCenter()
    {
        GameManager.Instance.OnMonsterAttackCenter(_realAtk);

        _transform.position = _spawnPosition;
    }

    public override void Die(bool isKillByPlayer)
    {
        _deadAction?.Invoke(this);

        if (isKillByPlayer)
        {
            SoundManager.Instance.PlaySFX(SFXType.Evt_MonsterDie);
            SpawnGoldFlyEffects();
            GetGold();
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
        MonsterPool.Instance.ReleaseBossMonster(this);
        MonsterManager.Instance.Unregister(this);
    }
}
