using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private GameObject _upgradePopup;

    public void OnStartButtonClick()
    {
        GameManager.Instance.StartGame();
    }

    public void OnUpgradeButtonClick()
    {
        _upgradePopup.SetActive(true);
        GameManager.Instance.SetGameState(GameState.Upgrade);
    }

    public void ExitButtonClick()
    {
        UpgradeManager.Instance.SaveUpgradeData();
        DataManager.SaveGoldData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
