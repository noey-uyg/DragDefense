using DG.Tweening;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class GameManager : DontDestroySingleton<GameManager>
{
    private GameState _state;

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Center _center;
    [SerializeField] private Circle _circle;
    [SerializeField] private TitlePanel _titlePanel;
    [SerializeField] private UpgradePanel _upgradePanel;
    [SerializeField] private MainHUD _mainHUD;

    private Vector3 _camOriginPos;
    private float _playStartTime;
    private BigInteger _goldAtStart;

    public GameState CurrentState { get { return _state; } }
    public Center Center { get { return _center; } }
    public Circle Circle { get { return _circle; } }

    protected override void OnAwake()
    {
        base.OnAwake();

        if(_mainCamera == null) _mainCamera = Camera.main;
        _camOriginPos = _mainCamera.transform.position;

        DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
        DOTween.SetTweensCapacity(500, 50);
        CSVParser.AllCSVLoad();
    }

    private void Start()
    {
        Init();
        OnLobby();
    }
    
    private void Init()
    {
        //InitStatRefresh();
        InitLoadData();
    }

    private void InitStatRefresh()
    {
        PlayerStat.RefreshStats();
        SkillStat.RefreshSkillStats();
        UpgradeManager.Instance.InitializeAllNodes();
    }

    private void InitLoadData()
    {
        DataManager.LoadGoldData();

        var (bgm, sfx) = DataManager.LoadSoundData();
        SoundManager.Instance.SetVolume(bgm, sfx);

        Language saveLang = DataManager.LoadLanguageData();
        if (saveLang == Language.None) LocalizationManager.Instance.SetDefaultLanguage();
        else LocalizationManager.Instance.ChangeLanguage(saveLang);
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
        ShakeCamera();
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
        SkillStat.RefreshSkillStats();
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

    public void ShakeCamera(float duration = 0.2f, float strength = 0.05f, int vivrato = 1)
    {
        if (_mainCamera == null) return;

        _mainCamera.transform.DOKill();
        _mainCamera.transform.position = _camOriginPos;

        _mainCamera.transform.DOShakePosition(duration, strength, vivrato)
            .OnComplete(()=>_mainCamera.transform.position = _camOriginPos);
    }
}
