using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _currencyText;
    [SerializeField] private TextMeshProUGUI _costText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _offsetY = 60f;

    public void Show(string title, string desc, string curStat, string cost, int curLevel, int maxLevel, Vector2 position, Vector2 canvasSize)
    {
        gameObject.SetActive(true);
        _titleText.text = title;
        _descText.text = desc;
        _currencyText.text = curStat;
        _costText.text = cost;
        _levelText.text = $"Lv. {curLevel} / {maxLevel}";

        _rectTransform.anchoredPosition = position + new Vector2(0, _offsetY);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);

        ClampToScreen(position, canvasSize);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void ClampToScreen(Vector2 nodeLoaclPos, Vector2 canvasSize)
    {
        Vector2 pos = _rectTransform.anchoredPosition;
        Vector2 size = _rectTransform.rect.size;
        Vector2 pivot = _rectTransform.pivot;

        float maxY = pos.y + (size.y * (1f - pivot.y));

        if(maxY > canvasSize.y / 2) pos.y = nodeLoaclPos.y - _offsetY - size.y;

        float minX = pos.x - (size.x * pivot.x);
        float maxX = pos.x + (size.x * (1f - pivot.x));

        if (minX < -canvasSize.x / 2) pos.x += (-canvasSize.x / 2 - minX);
        if (maxX > canvasSize.x / 2) pos.x -= (maxX - canvasSize.x / 2);

        float minY = pos.y - (size.y * pivot.y);
        if (minY < -canvasSize.y / 2) pos.y += (-canvasSize.y / 2 - minY);

        _rectTransform.anchoredPosition = pos;
    }
}
