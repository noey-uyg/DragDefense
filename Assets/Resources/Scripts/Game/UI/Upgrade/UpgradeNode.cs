using NUnit.Framework.Constraints;
using System.ComponentModel;
using System.Numerics;
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
    [TextArea(3, 5)] public string Description;
    public float GridX;
    public float GridY;

    [Header("Status")]
    public int level;
    public int MaxLevel;
    public string[] cost;
    public float[] Value;

    [Header("Connection")]
    public int connectID;
    public bool connectMax;

    public BigInteger GetCurrentCost()
    {
        if (level >= MaxLevel) return 0;

        int nextLevel = level + 1;
        if(nextLevel > MaxLevel || string.IsNullOrEmpty(cost[nextLevel])) return 0;

        return BigInteger.Parse(cost[nextLevel]);
    }
}

public class UpgradeNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UpgradeData _upgradeData;

    [Header("Node Set")]
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

    public void SetData(UpgradeData data)
    {
        _upgradeData = data;
        gameObject.name = $"Node_{data.ID}";
    }

    public void RefreshNodeStatus()
    {
        if (_upgradeData.connectID == 0)
        {
            SetActivate(true);
            return;
        }

        bool isParentClear = UpgradeManager.Instance.CheckIfCleared(_upgradeData.connectID, _upgradeData.connectMax);
        SetActivate(isParentClear);
    }

    private void SetActivate(bool active)
    {
        gameObject.SetActive(active);
        UpdateNodeImage();
        //SetActivateLine(active);
    }

    //private void SetActivateLine(bool active)
    //{
    //    if (_lines == null || _lines.Length == 0) return;

    //    for(int i=0;i<_lines.Length;i++)
    //    {
    //        _lines[i].SetActive(active);
    //    }
    //}

    public void OnUpgradeClick()
    {
        if (_upgradeData.level >= _upgradeData.MaxLevel) return;

        BigInteger curCost = _upgradeData.GetCurrentCost();

        if (PlayerStat.CurGold < curCost) return;

        PlayerStat.CurGold -= curCost;
        _upgradeData.level++;
        float bonusValue = _upgradeData.Value[_upgradeData.level];

        UpdateNodeImage();

        UpgradeManager.Instance.ApplyUpgrade(_upgradeData.Type, _upgradeData.ID, bonusValue, _upgradeData.level);
        UpgradeManager.Instance.NotifyNodeCleared();
        DataManager.SaveGoldData();
        UpgradeManager.Instance.SaveUpgradeData();
        OnPointerEnter(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isHover = true;
        UpdateNodeImage();

        bool isMax = _upgradeData.level >= _upgradeData.MaxLevel;

        string costStr = isMax ?
            "<color=red>(Max)</color>" :
            CurrencyFomatter.FormatBigInt(_upgradeData.GetCurrentCost());

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
