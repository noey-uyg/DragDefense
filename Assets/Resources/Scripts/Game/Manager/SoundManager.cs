using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SFXEntry
{
    public SFXType type;
    public AudioClip prefab;
}

[System.Serializable]
public class BGMEntry
{
    public BGMType type;
    public AudioClip prefab;
}

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmPlayer;
    [SerializeField] private GameObject _sfxPlayerPrefab;
    [SerializeField] private List<BGMEntry> _bgmEntries;
    [SerializeField] private List<SFXEntry> _sfxEntries;

    private Dictionary<BGMType, AudioClip> _bgmClip = new Dictionary<BGMType, AudioClip>();
    private Dictionary<SFXType, AudioClip> _sfxClip = new Dictionary<SFXType, AudioClip>();
    private Dictionary<SFXType, float> _sfxCooldowns = new Dictionary<SFXType, float>();
    private Stack<AudioSource> _unusedSource = new Stack<AudioSource>();
    private List<AudioSource> _activeSource = new List<AudioSource>();

    private int _initPoolSize = 20;
    private int _maxPoolSIze = 50;
    private float _bgmVolume = 0.5f;
    private float _sfxVolume = 0.5f;

    private const float _minSfxInterval = 0.05f;

    public float BGMVolume { get { return _bgmVolume; } }
    public float SFXVolume { get { return _sfxVolume; } }

    protected override void OnAwake()
    {
        base.OnAwake();
        Init();
    }

    private void Init()
    {
        foreach(var v in _bgmEntries)
        {
            _bgmClip.Add(v.type, v.prefab);
        }

        foreach(var v in _sfxEntries)
        {
            _sfxClip.Add(v.type, v.prefab);
        }

        for(int i = 0; i < _initPoolSize; i++)
        {
            CreateAudioSource();
        }
    }

    private AudioSource CreateAudioSource()
    {
        if (_unusedSource.Count + _activeSource.Count >= _maxPoolSIze) return null;

        var go = Instantiate(_sfxPlayerPrefab, transform);
        AudioSource source = go.GetComponent<AudioSource>();
        if(source == null) source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        _unusedSource.Push(source);

        return source;
    }

    private AudioSource GetAvailableSource()
    {
        // 사용된 오디오 정리
        for (int i = _activeSource.Count - 1; i >= 0; i--)
        {
            if (!_activeSource[i].isPlaying)
            {
                _unusedSource.Push(_activeSource[i]);
                _activeSource.RemoveAt(i);
            }
        }

        // 사용 가능 소스 반환
        if(_unusedSource.Count > 0)
        {
            AudioSource source = _unusedSource.Pop();
            _activeSource.Add(source);
            return source;
        }

        // 사용 소스 없을 때 새로 생성
        if(_unusedSource.Count + _activeSource.Count < _maxPoolSIze)
        {
            AudioSource source = CreateAudioSource();
            if(source != null)
            {
                _unusedSource.Pop();
                _activeSource.Add(source);
                return source;
            }
        }

        // 모두 사용되고 있을 때
        AudioSource old = _activeSource[0];
        old.Stop();

        _activeSource.RemoveAt(0);
        _activeSource.Add(old);

        return old;
    }

    public void PlayBGM(BGMType type)
    {
        if (!_bgmClip.TryGetValue(type, out var clip)) return;

        if (_bgmPlayer.clip == clip && _bgmPlayer.isPlaying) return;

        _bgmPlayer.clip = clip;
        _bgmPlayer.loop = true;
        _bgmPlayer.volume = _bgmVolume;
        _bgmPlayer.Play();
    }

    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }

    public void PlaySFX(SFXType type)
    {
        if (!_sfxClip.TryGetValue(type, out var clip)) return;

        if(_sfxCooldowns.TryGetValue(type, out var lastTime))
        {
            if (Time.time - lastTime < _minSfxInterval) return;
        }

        var source = GetAvailableSource();

        if(source == null) return;

        source.clip = clip;
        source.volume = _sfxVolume;
        source.loop = false;

        _sfxCooldowns[type] = Time.time;
        source.Play();
    }

    public void SetBGMVolume(float vol)
    {
        _bgmVolume = Mathf.Clamp01(vol);
        _bgmPlayer.volume = _bgmVolume;
    }

    public void SetSFXVolume(float vol)
    {
        _sfxVolume = Mathf.Clamp01(vol);
    }

    public void SetVolume(float bgm, float sfx)
    {
        _bgmVolume = Mathf.Clamp01(bgm);
        _bgmPlayer.volume = _bgmVolume;
        _sfxVolume = Mathf.Clamp01(sfx);
    }
}
