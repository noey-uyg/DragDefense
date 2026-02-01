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
    private List<AudioSource> _sfxPool = new List<AudioSource>();

    private int _initPoolSize = 20;
    private float _bgmVolume = 0.5f;
    private float _sfxVolume = 0.5f;

    private const float _minSfxInterval = 0.05f;

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
        var go = Instantiate(_sfxPlayerPrefab, transform);
        AudioSource source= go.AddComponent<AudioSource>();
        source.playOnAwake = false;
        _sfxPool.Add(source);

        return source;
    }

    private AudioSource GetAvailableSource()
    {
        for(int i = 0; i < _sfxPool.Count; i++)
        {
            if (!_sfxPool[i].isPlaying) return _sfxPool[i];
        }

        return CreateAudioSource();
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
}
