using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private Transform _transform;

    private float _curRadius;
    private int _curAtk;
    private float _curDelay;

    private float _timer = 0f;

    public float Radius { get { return _curRadius; } }
    public int AtkDamage { get { return _curAtk; } }
    public float DamageDleay { get { return _curDelay; } }
    public Transform GetTransform {  get { return _transform; } }

    public void Init()
    {
        _curRadius = PlayerStat.CurRadius;
        _curDelay = PlayerStat.CurAtkDelay;
        _curAtk = Mathf.RoundToInt(PlayerStat.CurAtk);

        float diameter = _curRadius * 2f;
        _transform.localScale = new Vector3(diameter, diameter, 1f);

        _timer = 0f;
    }

    public bool IsReady()
    {
        _timer += Time.deltaTime;

        return _timer >= _curDelay;
    }

    public void ResetTimer()
    {
        _timer = 0f;
    }
}
