using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [SerializeField] protected int _monsterID;
    [SerializeField] protected Rigidbody2D _rigidBody;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Transform _transform;
    
    [SerializeField] protected int _rewardGold;

    protected float _baseHP = 5;
    protected float _baseAtk = 1;
    protected float _baseSpeed = 1;

    private float _realHP;
    private float _realAtk;
    private float _realSpeed;
    private Vector2 _centerPosition = Vector2.zero;
    private Action<BaseMonster> _deadAction;

    public int MonsterID { get { return _monsterID; } }
    public Transform GetTransform { get { return _transform; } }

    public void MoveToTarget(Vector2 targetPos, float deltaTime)
    {
        Vector2 currentPos = _transform.position;
        Vector2 dir = targetPos - currentPos;
        float distance = dir.magnitude;

        if(distance > 0.1f)
        {
            Vector2 toCenterDir = dir / distance;

            Vector2 tangetDir = new Vector2(toCenterDir.y, -toCenterDir.x);

            float approachWeight = 0.5f;
            float orbitWeight = 1.0f;

            Vector2 moveDir = (toCenterDir * approachWeight) + (tangetDir * orbitWeight);
            moveDir = moveDir.normalized;

            Vector2 nextPos = currentPos + moveDir * _realSpeed * deltaTime;
            _transform.position = nextPos;
        }
    }

    public void Init()
    {
        MonsterManager.Instance.Register(this);
        _deadAction = null;
        _realHP = _baseHP;
        _realAtk = _baseAtk;
        _realSpeed = _baseSpeed;
    }

    public void TakeDamage(float dam)
    {
        _realHP -= dam;
        if (_realHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        _deadAction?.Invoke(this);
        MonsterPool.Instance.ReleaseNormalMonster(this);
        MonsterManager.Instance.Unregister(this);
    }

    public void SetDeadAction(Action<BaseMonster> action)
    {
        _deadAction = action;
    }
}
