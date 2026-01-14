using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _goldText;

    private Coroutine _timerCoroutine;
    private readonly WaitForSeconds _timerWFS = new WaitForSeconds(0.01f);
    private readonly StringBuilder _sb = new StringBuilder(16);

    private void OnEnable()
    {
        PlayerStat.OnGoldChanged += UpdateGoldUI;
        UpdateGoldUI(PlayerStat.CurGold);

        StopTimer();
        _timerCoroutine = StartCoroutine(IETimerRoutine());
    }

    private void OnDisable()
    {
        PlayerStat.OnGoldChanged -= UpdateGoldUI;
        StopTimer();
    }

    private void StopTimer()
    {
        if(_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
    }

    private IEnumerator IETimerRoutine()
    {
        float remainingTime = PlayerStat.CurPlayTime;
        float lastTime = Time.time;

        while(remainingTime > 0)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing)
            {
                lastTime = Time.time;
                yield return null;
                continue;
            }

            float deltaTime = Time.time - lastTime;
            remainingTime -= deltaTime;
            lastTime = Time.time;

            if (remainingTime < 0) remainingTime = 0;

            _sb.Clear();
            _sb.Append(remainingTime.ToString("F2"));
            _timerText.SetText(_sb);

            yield return _timerWFS;
        }

        _timerText.SetText("0.00");
        GameManager.Instance.SetGameState(GameState.GameOver);
    }

    private void UpdateGoldUI(int gold)
    {
        _goldText.text = gold.ToString("N0");
    }
}
