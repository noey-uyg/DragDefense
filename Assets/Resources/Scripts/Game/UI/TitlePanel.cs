using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private GameObject _mainUIPanel;
    [SerializeField] private GameObject _upgradePopup;

    public void OnStartButtonClick()
    {
        GameManager.Instance.StartGame();
    }

    public void OnUpgradeButtonClick()
    {
        _upgradePopup.SetActive(true);
    }

    private void OnDisable()
    {
        _mainUIPanel.SetActive(true);
    }
}
