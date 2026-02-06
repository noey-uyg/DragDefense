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
    [SerializeField] protected Color _normalDamColor;
    [SerializeField] protected Color _criticalDamColor;
    [SerializeField] protected Material _hitMaterial;
    [SerializeField] protected Material _originMaterial;

    protected float _realHP;
    protected float _realAtk;
    protected float _realSpeed;
    protected float _rewardGold;
    protected float _visualRadius;
    protected float _orbitalHitTimer = 0f;
    protected float _orbitalHitInterval = 0.5f;

    protected MonsterData _monsterData;
    protected Action<BaseMonster> _deadAction;

    protected Coroutine _flashCoroutine;
    protected WaitForSeconds _fwfs = new WaitForSeconds(0.05f);


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

    public virtual void Init(MonsterData data)
    {
        _monsterData = data;
        _monsterID = data.monsterID;

        Sprite sprite = SpriteAtlasManager.Instance.GetSprite("Monsters_Atlas", data.sprite.name);
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.material = _originMaterial;
        _spriteRenderer.sortingOrder = data.monsterLevel;

        _transform.localScale = new Vector3(data.baseScale, data.baseScale, 1);
        _visualRadius = _spriteRenderer.bounds.extents.x;

        _realHP = data.baseHP;
        _realAtk = data.baseAtk;
        _realSpeed = data.baseSpeed;
        _rewardGold = data.rewardGold;

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
            _flashCoroutine = null;
        }

        MonsterManager.Instance.Register(this);
        _deadAction = null;
    }

    public void TakeDamage(float dam, bool isCritical = false)
    {
        SoundManager.Instance.PlaySFX(SFXType.Evt_MonsterHit);

        _realHP -= dam;
        ApplyVampire();
        ShowDamageText(dam, isCritical);
        if (_realHP <= 0)
        {
            Die(true);
            return;
        }

        if (gameObject.activeInHierarchy)
        {
            PlayHitFlash();
        }
    }

    private void ShowDamageText(float dam, bool isCritical = false)
    {
        TextEffect te = EffectUIPool.Instance.Get<TextEffect>();

        if (te != null)
        {
            Color finalColor = isCritical ? Color.greenYellow : Color.yellow;

            te.Show(Mathf.RoundToInt(dam).ToString(), _transform.position, finalColor);
        }
    }

    protected virtual void AttackCenter()
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

    public virtual void Die(bool isKillByPlayer)
    {
        MonsterManager.Instance.Unregister(this);
        _deadAction?.Invoke(this);

        if (isKillByPlayer)
        {
            SoundManager.Instance.PlaySFX(SFXType.Evt_MonsterDie);
            SpawnGoldFlyEffects();
            GetGold();
        }
        if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
        MonsterPool.Instance.ReleaseNormalMonster(this);
    }

    protected void GetGold()
    {
        int finalGold = Mathf.RoundToInt(_rewardGold * PlayerStat.CurGoldGainPercent);

        if (Random.Range(0f, 100f) <= PlayerStat.CurGoldBonusChance) finalGold *= 2;

        PlayerStat.CurGold += finalGold;
    }

    public void SetDeadAction(Action<BaseMonster> action)
    {
        _deadAction = action;
    }

    protected void SpawnGoldFlyEffects()
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

    private void ApplyVampire()
    {
        if(Random.Range(0f,100f) <= PlayerStat.CurVampire)
        {
            GameManager.Instance.Center.Heal(1);
        }
    }

    public void UpdateOrbitalTimer(float dt)
    {
        if (_orbitalHitTimer > 0) _orbitalHitTimer -= dt;
    }

    public bool CanHitByOrbital() => _orbitalHitTimer <= 0f;

    public void ResetOrbitalHitTimer()
    {
        _orbitalHitTimer = _orbitalHitInterval;
    }
}
