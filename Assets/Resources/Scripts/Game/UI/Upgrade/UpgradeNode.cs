using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeData
{
    [Header("Info")]
    public UpgradeType Type;
    public int ID;
    public string Name;
    [TextArea(3,5)] public string Description;

    [Header("Status")]
    public int level;
    public int MaxLevel;
    public int[] cost;
    public float[] Value;

    [Header("Connection")]
    public int connectID;
}

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UpgradeData _upgradeData;
    [SerializeField] private Button _nodeButton;
    [SerializeField] private Image _nodeImage;
    [SerializeField] private Transform _transform;

    [Header("Sprites")]
    [SerializeField] private Sprite _normalSprite;
    [SerializeField] private Sprite _highlightSprite;
    [SerializeField] private Sprite _lockSprite;
    [SerializeField] private Sprite _lockHighlightSprite;

    private bool _isHover = false;

    public UpgradeData UpgradeData { get { return _upgradeData; } }

    public void RefreshNodeStatus()
    {
        if (_upgradeData.connectID == 0)
        {
            SetActivate(true);
            return;
        }

        bool isParentClear = UpgradeManager.Instance.CheckIfCleared(_upgradeData.connectID);
        SetActivate(isParentClear);
    }

    private void SetActivate(bool active)
    {
        gameObject.SetActive(active);
        UpdateNodeImage();
    }

    public void OnUpgradeClick()
    {
        if (_upgradeData.level >= _upgradeData.MaxLevel) return;

        float bonusValue = _upgradeData.Value[_upgradeData.level];
        _upgradeData.level++;

        UpdateNodeImage();

        UpgradeManager.Instance.ApplyUpgrade(_upgradeData.Type, _upgradeData.ID, bonusValue);
        UpgradeManager.Instance.SaveUpgradeData();
        UpgradeManager.Instance.NotifyNodeCleared();
        DataManager.SaveGoldData();
        OnPointerEnter(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHover = true;
        UpdateNodeImage();

        bool isMax = _upgradeData.level >= _upgradeData.MaxLevel;

        int cost = isMax ? 0 : _upgradeData.cost[_upgradeData.level];
        string costStr = isMax ? "<color=red>(Max)</color>" : cost.ToString();

        string finalDesc = _upgradeData.Description;
        float curTotalValue = _upgradeData.Value[_upgradeData.level];

        if (!isMax)
        {
            float nextTotalValue = _upgradeData.Value[_upgradeData.level + 1];
            float additionalValue = nextTotalValue - curTotalValue;

            finalDesc = string.Format(_upgradeData.Description, additionalValue);
        }
        else
        {
            finalDesc = string.Format(_upgradeData.Description, 0);
        }

        TooltipManager.Instance.ShowUpgradeTooltip(
            _upgradeData.Name,
            finalDesc,
            costStr,
            _upgradeData.level,
            _upgradeData.MaxLevel,
            _transform.position
            );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHover = false;
        UpdateNodeImage();
        TooltipManager.Instance.HideTooltip();
    }

    private void UpdateNodeImage()
    {
        bool isLocked = _upgradeData.level == 0;

        if (isLocked) _nodeImage.sprite = _isHover ? _lockHighlightSprite : _lockSprite;
        else _nodeImage.sprite = _isHover ? _highlightSprite : _normalSprite;
    }
}
