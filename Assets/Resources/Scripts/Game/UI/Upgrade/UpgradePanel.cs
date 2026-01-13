using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    private void Start()
    {
        UpgradeManager.Instance.InitializeAllNodes(this.gameObject);
    }

    public void OnClickExit()
    {
        gameObject.SetActive(false);
    }
}
