using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    [SerializeField] private GameObject _upgradePopup;

    public void OnStartButtonClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        GameManager.Instance.StartGame();
    }

    public void OnUpgradeButtonClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        _upgradePopup.SetActive(true);
        GameManager.Instance.SetGameState(GameState.Upgrade);
    }

    public void OnSettingButtonClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        PopupManager.Instance.ShowPopup<SettingPopup>(popup =>
        {
            popup.Init();
        });
    }

    public void ExitButtonClick()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        UpgradeManager.Instance.SaveUpgradeData();
        DataManager.SaveGoldData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
