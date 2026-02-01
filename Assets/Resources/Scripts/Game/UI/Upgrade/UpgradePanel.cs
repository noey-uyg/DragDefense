using System.Numerics;
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

    public void OnClickStart()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        gameObject.SetActive(false);
        GameManager.Instance.StartGame();
    }

    public void OnClickMain()
    {
        SoundManager.Instance.PlaySFX(SFXType.UI_ButtonClick);
        gameObject.SetActive(false);
        GameManager.Instance.SetGameState(GameState.Lobby);
    }

    private void UpdateGoldUI(BigInteger gold)
    {
        _goldText.text = CurrencyFomatter.FormatBigInt(gold);
    }
}
