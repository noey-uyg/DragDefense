using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Center : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [SerializeField] private Material _hitMaterial;
    [SerializeField] private Material _originMaterial;
    private Coroutine _flashCoroutine;
    private WaitForSeconds _fwfs = new WaitForSeconds(0.05f);

    private float _maxHP;
    private float _currentHP;
    private float _defense;
    private float _visualRadius;

    public float VisualRadius { get { return _visualRadius; } }

    public void Init()
    {
        _maxHP = PlayerStat.CurMaxHP;
        _currentHP = _maxHP;
        _defense = PlayerStat.CurDamageReduction;
        _spriteRenderer.material = _originMaterial;
        _visualRadius = _spriteRenderer.bounds.extents.x;

        if(_hpSlider != null)
        {
            _hpSlider.maxValue = _maxHP;
            _hpSlider.value = _currentHP;
        }
    }

    public void TakeDamage(float monsterAtk)
    {
        SoundManager.Instance.PlaySFX(SFXType.Evt_CenterHit);

        float fianlDamage = Mathf.Max(1f, monsterAtk - _defense);
        _currentHP -= fianlDamage;

        if(_hpSlider != null)
        {
            _hpSlider.value = _currentHP;
        }

        PlayHitFlash();

        if (_currentHP <= 0f)
        {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
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

    public void Heal(float amount)
    {
        if (_currentHP <= 0f) return;

        _currentHP += amount;

        if (_currentHP > _maxHP) _currentHP = _maxHP;

        if (_hpSlider != null)
        {
            _hpSlider.value = _currentHP;
        }
    }
}
