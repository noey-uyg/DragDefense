using System.Collections.Generic;
using UnityEngine;

public class UpgradeTreeConnector : MonoBehaviour
{
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private Transform _parent;

    private Dictionary<int, RectTransform> nodeRects = new Dictionary<int, RectTransform>();

    public void SetNodeRect(int id, RectTransform rectTransform)
    {
        if (nodeRects.ContainsKey(id)) return;

        nodeRects[id] = rectTransform;
    }
    
    public void CreateAllConnection(List<UpgradeNode> datas)
    {
        foreach(var v in datas)
        {
            if(v.UpgradeData.connectID != 0 && nodeRects.ContainsKey(v.UpgradeData.connectID))
            {
                DrawLine(nodeRects[v.UpgradeData.connectID], nodeRects[v.UpgradeData.ID]);
            }
        }
    }

    private void DrawLine(RectTransform start, RectTransform end)
    {
        GameObject lineObj = Instantiate(_linePrefab, _parent);
        lineObj.name = $"Line_{start.name}_to_{end.name}";
        
        RectTransform lineRect = lineObj.GetComponent<RectTransform>();

        Vector2 startPos = start.anchoredPosition;
        Vector2 endPos = end.anchoredPosition;

        Vector2 direction = endPos - startPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        lineRect.sizeDelta = new Vector2(distance, 5f);
        lineRect.anchorMin = start.anchorMin;
        lineRect.anchorMax = start.anchorMax;
        lineRect.pivot = new Vector2(0, 0.5f);
        lineRect.anchoredPosition = startPos;
        lineRect.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
