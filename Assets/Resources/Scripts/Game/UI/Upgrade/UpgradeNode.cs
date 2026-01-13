using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UpgradeData
{
    public int ID;
    public string Name;
    public string Description;
    public int level;
    public int MaxLevel;
    public int[] cost;
    public int[] Value;
    public int connectID;
}

public class UpgradeNode : MonoBehaviour
{
    [SerializeField] private UpgradeData _upgradeData;
    [SerializeField] private Button _nodeButton;
    [SerializeField] private Image _nodeImage;

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
    }

    public void OnUpgradeClick()
    {
        if (_upgradeData.level >= _upgradeData.MaxLevel) return;

        _upgradeData.level++;
        UpgradeManager.Instance.NotifyNodeCleared();
    }
}
