using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    private int _baseAtk = 10;
    private float _baseDamageDelay = 1f;

    private float _lastDamageTime = -999;
    private List<BaseMonster> _monsters = new List<BaseMonster>();

    private void Update()
    {
        if(Time.time - _lastDamageTime >= _baseDamageDelay)
        {
            for(int i=_monsters.Count-1; i>=0; i--)
            {
                _monsters[i].TakeDamage(_baseAtk);
            }
            
            _lastDamageTime = Time.time;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseMonster monster = collision.GetComponent<BaseMonster>();
        if(monster!= null && !_monsters.Contains(monster))
        {
            _monsters.Add(monster);
            monster.TakeDamage(_baseAtk);
            monster.SetDeadAction(RemoveMonster);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BaseMonster monster = collision.GetComponent<BaseMonster>();
        if (monster != null)
        {
            _monsters.Remove(monster);
        }
    }

    public void RemoveMonster(BaseMonster monster)
    {
        _monsters.Remove(monster);
        Debug.Log(_monsters.Count);
    }
}
