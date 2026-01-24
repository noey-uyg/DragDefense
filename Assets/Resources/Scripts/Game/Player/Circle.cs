using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private SpriteRenderer _sr;

    [SerializeField] private Color _attackColor;

    // 모션 시간
    private WaitForSeconds _wfs = new WaitForSeconds(0.1f);
    private float _scaleMultiplier = 1.1f;

    private float _curRadius;
    private int _curAtk;
    private float _curDelay;
    private float _timer = 0f;

    private Vector3 _baseScale;
    private Color _baseColor;
    private Coroutine _motionCoroutine;

    public float Radius { get { return _curRadius; } }
    public int AtkDamage { get { return _curAtk; } }
    public float DamageDleay { get { return _curDelay; } }
    public Transform GetTransform {  get { return _transform; } }

    private void Start()
    {
        _baseColor = _sr.color;
    }

    public void Init()
    {
        _curRadius = PlayerStat.CurRadius;
        _curDelay = PlayerStat.CurAtkDelay;
        _curAtk = Mathf.RoundToInt(PlayerStat.CurAtk);

        float diameter = _curRadius * 2f;
        _baseScale = new Vector3(diameter, diameter, 1f);
        _transform.localScale = _baseScale;
        _sr.color = _baseColor;

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

    public (int dam, bool isCri) GetCalcDamage()
    {
        int baseAtk = Mathf.RoundToInt(PlayerStat.CurAtk);
        bool isCri = Random.Range(0f, 100f) <= PlayerStat.CurCritical;
        Debug.Log($"{baseAtk}, {PlayerStat.CurAtk}");
        if (isCri)
        {
            int criDam = Mathf.RoundToInt(baseAtk * PlayerStat.CurCriticalDam);
            return (criDam, true);
        }

        return (baseAtk, false);
    }

    public void PlayAttackMotion()
    {
        if (_motionCoroutine != null) StopCoroutine(_motionCoroutine);

        _motionCoroutine = StartCoroutine(IEAttackMotion());

        Vector3 effectScale = new Vector3(_curRadius, _curRadius, 1);
        EffectManager.PlayEffect(EffectType.CircleHit, _transform.position, Quaternion.identity, effectScale);
    }

    private IEnumerator IEAttackMotion()
    {
        _transform.localScale = _baseScale * _scaleMultiplier;
        _sr.color = _attackColor;

        yield return _wfs;

        _transform.localScale = _baseScale;
        _sr.color = _baseColor;
        _motionCoroutine = null;
    }
}
