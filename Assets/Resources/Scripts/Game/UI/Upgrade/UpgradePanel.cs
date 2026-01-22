using TMPro;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _goldText;

    private void OnEnable()
    {
        PlayerStat.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(PlayerStat.CurGold);
    }

    private void OnDisable()
    {
        PlayerStat.OnGoldChanged -= UpdateGoldUI;
    }

    private void Start()
    {
        UpgradeManager.Instance.InitializeAllNodes(this.gameObject);
    }

    public void OnClickStart()
    {
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void OnClickMain()
    {
        gameObject.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Lobby);
    }

    private void UpdateGoldUI(int gold)
    {
        _goldText.text = gold.ToString("N0");
    }
}
