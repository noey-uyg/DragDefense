using UnityEngine;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private UpgradeTooltip _upgradeTooltip;
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private Camera _UICamera;

    private Vector2 _canvasRectSize;

    protected override void OnAwake()
    {
        base.OnAwake();
        _canvasRectSize = _canvasRect.rect.size;
    }

    public void ShowUpgradeTooltip(string title, string desc, string cost, int curLevel, int maxLevel, Vector2 position)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(_UICamera, position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            screenPos,
            _UICamera,
            out Vector2 localPos);

        _upgradeTooltip.Show(title, desc, cost, curLevel, maxLevel, localPos, _canvasRectSize);
    }

    public void HideTooltip()
    {
        _upgradeTooltip.Hide();
    }
}
