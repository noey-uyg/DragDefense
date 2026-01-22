using System;
using System.Collections;
using System.Threading;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Random = UnityEngine.Random;

public class BaseMonster : MonoBehaviour
{
    [SerializeField] protected int _monsterID;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Transform _transform;

    private float _realHP;
    private float _realAtk;
    private float _realSpeed;
    private float _rewardGold;
    private float _visualRadius;
    private Action<BaseMonster> _deadAction;

    [SerializeField] private Material _hitMaterial;
    [SerializeField] private Material _originMaterial;
    private Coroutine _flashCoroutine;
    private WaitForSeconds _fwfs = new WaitForSeconds(0.05f);

    [SerializeField] private Color _normalDamColor;
    [SerializeField] private Color _criticalDamColor;

    public int MonsterID { get { return _monsterID; } }
    public Transform GetTransform { get { return _transform; } }
    public float VisualRadius { get { return _visualRadius; } }

    public void MoveToTarget(Vector2 targetPos, float deltaTime)
    {
        Vector2 currentPos = _transform.position;
        Vector2 dir = targetPos - currentPos;
        float distance = dir.magnitude;

        float centerRadius = GameManager.Instance.Center.VisualRadius;
        float attackRange = centerRadius + _visualRadius;

        if(distance <= attackRange)
        {
            AttackCenter();
            return;
        }

        Vector2 toCenterDir = dir / distance;

        Vector2 tangetDir = new Vector2(toCenterDir.y, -toCenterDir.x);

        float approachWeight = 0.5f;
        float orbitWeight = 1.0f;

        Vector2 moveDir = (toCenterDir * approachWeight) + (tangetDir * orbitWeight);
        moveDir = moveDir.normalized;

        Vector2 nextPos = currentPos + moveDir * _realSpeed * deltaTime;
        _transform.position = nextPos;
    }

    public void Init(MonsterData data)
    {
        _monsterID = data.monsterID;
        _spriteRenderer.sprite = data.sprite;
        _spriteRenderer.material = _originMaterial;
        _spriteRenderer.sortingOrder = data.monsterLevel;

        _transform.localScale = new Vector3(data.baseScale, data.baseScale, 1);
        _visualRadius = _spriteRenderer.bounds.extents.x;

        _realHP = data.baseHP;
        _realAtk = data.baseAtk;
        _realSpeed = data.baseSpeed;
        _rewardGold = data.rewardGold;

        if(_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }

        MonsterManager.Instance.Register(this);
        _deadAction = null;
    }

    public void TakeDamage(float dam)
    {
        _realHP -= dam;

        if (_realHP <= 0)
        {
            Die(true);
            return;
        }

        PlayHitFlash();
        ShowDamageText(dam);
    }

    private void ShowDamageText(float dam)
    {
        TextEffect te = EffectUIPool.Instance.Get<TextEffect>();

        if (te != null)
        {
            te.Show(Mathf.RoundToInt(dam).ToString(), _transform.position, Color.yellow);
        }
    }

    private void AttackCenter()
    {
        GameManager.Instance.OnMonsterAttackCenter(_realAtk);
        Die(false);
    }

    private void PlayHitFlash()
    {
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        _flashCoroutine = StartCoroutine(IEHitFlash());
    }

    private IEnumerator IEHitFlash()
    {
        _spriteRenderer.material = _hitMaterial;
        
        yield return _fwfs;

        _spriteRenderer.material = _originMaterial;
        _flashCoroutine = null;
    }

    public void Die(bool isKillByPlayer)
    {
        _deadAction?.Invoke(this);

        if (isKillByPlayer)
        {
            SpawnGoldFlyEffects();

            int finalGold = Mathf.RoundToInt(_rewardGold * PlayerStat.CurGoldGainPercent);
            PlayerStat.CurGold += finalGold;
        }

        MonsterPool.Instance.ReleaseNormalMonster(this);
        MonsterManager.Instance.Unregister(this);
    }

    public void SetDeadAction(Action<BaseMonster> action)
    {
        _deadAction = action;
    }

    private void SpawnGoldFlyEffects()
    {
        Vector3 targetPos = MainHUD.Instance.GoldIconWorldPosition;

        int count = Math.Max(1, Random.Range(0, (int)PlayerStat.CurMonsterLevel));

        for(int i = 0; i < count; i++)
        {
            var ge = EffectUIPool.Instance.Get<GoldEffect>();
            if (ge != null)
            {
                Vector3 startPos = _transform.position + (Vector3)Random.insideUnitCircle * 0.3f;
                ge.Show(startPos, targetPos);
            }
        }
    }
}
