using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    [SerializeField] protected int _monsterID;
    protected float _baseHP = 100;
    protected float _baseAtk = 1;
    protected float _baseSpeed = 1;

    Action<BaseMonster> _deadAction;

    public int MonsterID { get { return _monsterID; } }

    public void Init()
    {
        _deadAction = null;
    }

    public void TakeDamage(float dam)
    {
        _baseHP -= dam;

        Debug.Log("dam");
        if (_baseHP <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Dead");
        _deadAction?.Invoke(this);
        Destroy(gameObject);
    }

    public void SetDeadAction(Action<BaseMonster> action)
    {
        _deadAction = action;
    }
}
