using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    private List<UpgradeNode> _allNodes = new List<UpgradeNode>();

    public void InitializeAllNodes(GameObject panel)
    {
        UpgradeNode[] nodes = panel.GetComponentsInChildren<UpgradeNode>();

        foreach (UpgradeNode node in nodes)
        {
            if(!_allNodes.Contains(node))
                _allNodes.Add(node);
        }

        LoadUpgradeData();

        NotifyNodeCleared();
    }

    public bool CheckIfCleared(int id)
    {
        foreach(var node in _allNodes)
        {
            if(node.UpgradeData.ID == id)
            {
                return node.UpgradeData.level > 0;
            }
        }

        return false;
    }

    public void NotifyNodeCleared()
    {
        foreach(var node in _allNodes)
        {
            node.RefreshNodeStatus();
        }
    }

    public void ApplyUpgrade(UpgradeType type, int uniqueID, float value)
    {
        PlayerStat.UpdateContiribution(type, uniqueID, value);
        DataManager.SaveGoldData();
    }

    public void SaveUpgradeData()
    {
        UpgradeSaveData saveData = new UpgradeSaveData();

        foreach(var node in _allNodes)
        {
            saveData.datas.Add(new NodeSaveData
            {
                id = node.UpgradeData.ID,
                level = node.UpgradeData.level
            });
        }

        DataManager.SaveUpgradeData(saveData);
    }

    public void LoadUpgradeData()
    {
        UpgradeSaveData saveData = DataManager.LoadUpgradeData();

        if (saveData == null) return;

        foreach (var v in saveData.datas)
        {
            UpgradeNode target = _allNodes.Find(x => x.UpgradeData.ID == v.id);
            if(target != null)
            {
                target.UpgradeData.level = v.level;

                if (v.level > 0)
                {
                    float val = target.UpgradeData.Value[v.level];
                    ApplyUpgrade(target.UpgradeData.Type, target.UpgradeData.ID, val);
                }
            }
        }
    }
}
