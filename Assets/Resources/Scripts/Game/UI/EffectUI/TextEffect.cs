using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextEffect : EffectUI
{
    [SerializeField] private TextMeshProUGUI _text;

    public void Show(string msg, Vector3 worldPos, Color color)
    {
        Init(worldPos);

        _text.text = msg;
        _text.color = color;
        _text.alpha = 1f;

        _sequence = DOTween.Sequence()
            .SetAutoKill(true)
            .OnComplete(OnReleaseToPool);

        float tempY = _transform.position.y + Random.Range(0f, 0.8f);
        _sequence.Join(_transform.DOMoveY(tempY, 0.2f).SetEase(Ease.OutQuart));
        _sequence.Join(_transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f));
        _sequence.AppendInterval(0.2f);
        _sequence.Append(_text.DOFade(0f, 0.2f));
    }

    protected override void OnReleaseToPool()
    {
        EffectUIPool.Instance.Release(this);
    }
}
