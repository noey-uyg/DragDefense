using UnityEngine;
using static UnityEngine.CullingGroup;
using UnityEngine.SceneManagement;

public class GameManager : DontDestroySingleton<GameManager>
{
    private GameState _state;

    [SerializeField] private Center _center;
    [SerializeField] private Circle _circle;
    [SerializeField] private GameObject _titlePanel;

    public GameState CurrentState { get { return _state; } }

    public void SetGameState(GameState state)
    {
        if (_state == state) return;

        _state = state;
        ChagngeGameState();
    }

    public void StartGame()
    {
        SetGameState(GameState.Playing);
    }

    private void ChagngeGameState()
    {
        switch (_state)
        {
            case GameState.Playing:
                PlayerStat.RefreshStats();
                _center.gameObject.SetActive(true);
                _center.Init();
                _circle.gameObject.SetActive(true);
                _circle.Init();
                _titlePanel.SetActive(false);
                break;
            default:
                _center.gameObject.SetActive(false);
                _circle.gameObject.SetActive(false);
                _titlePanel.SetActive(true);
                break;

        }
    }
}
