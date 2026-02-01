using DG.Tweening;
using System.Numerics;
using UnityEngine;

public class GameManager : DontDestroySingleton<GameManager>
{
    private GameState _state;

    [SerializeField] private Center _center;
    [SerializeField] private Circle _circle;
    [SerializeField] private TitlePanel _titlePanel;
    [SerializeField] private UpgradePanel _upgradePanel;
    [SerializeField] private MainHUD _mainHUD;

    private float _playStartTime;
    private BigInteger _goldAtStart;

    public GameState CurrentState { get { return _state; } }
    public Center Center { get { return _center; } }
    public Circle Circle { get { return _circle; } }

    protected override void OnAwake()
    {
        base.OnAwake();

        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
        DOTween.SetTweensCapacity(500, 50);
        CSVParser.AllCSVLoad();
    }

    private void Start()
    {
        UpgradeManager.Instance.InitializeAllNodes();
        DataManager.LoadGoldData();
        OnLobby();
    }

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
            case GameState.Playing: OnPlaying(); break;
            case GameState.GameOver: OnGameOver(); break;
            case GameState.Lobby: OnLobby(); break;
        }
    }

    public void OnMonsterAttackCenter(float damage)
    {
        _center.TakeDamage(damage);
    }

    public void OnUpgradePanel()
    {
        SoundManager.Instance.PlayBGM(BGMType.Main);
        _titlePanel.OnUpgradeButtonClick();
    }

    private void OnPlaying()
    {
        MonsterManager.Instance.ClearAllMonsters();
        SpawnManager.Instance.ResetBossState();
        PlayerStat.RefreshStats();
        SoundManager.Instance.PlayBGM(BGMType.Game);

        _center.gameObject.SetActive(true);
        _center.Init();

        _circle.gameObject.SetActive(true);
        _circle.Init();

        SkillManager.Instance.Init();

        _titlePanel.gameObject.SetActive(false);
        _mainHUD.gameObject.SetActive(true);

        _playStartTime = Time.time;
        _goldAtStart = PlayerStat.CurGold;
    }

    private void OnGameOver()
    {
        MonsterManager.Instance.ClearAllMonsters();
        SpawnManager.Instance.ResetBossState();
        SkillManager.Instance.CleanUp();

        _circle.gameObject.SetActive(false);
        _mainHUD.gameObject.SetActive(false);

        float totalSurvivalTime = Mathf.Min(Time.time - _playStartTime, PlayerStat.CurPlayTime);
        BigInteger earnedGold = PlayerStat.CurGold - _goldAtStart;

        DataManager.SaveGoldData();

        PopupManager.Instance.ShowPopup<ResultPopup>(popup =>
        {
            popup.Init(earnedGold, totalSurvivalTime);
        });
    }

    private void OnLobby()
    {
        MonsterManager.Instance.ClearAllMonsters();
        SoundManager.Instance.PlayBGM(BGMType.Main);

        _center.gameObject.SetActive(false);
        _circle.gameObject.SetActive(false);
        _mainHUD.gameObject.SetActive(false);
        _titlePanel.gameObject.SetActive(true);
    }
}
