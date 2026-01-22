using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Pool;

public class EffectUIPool : Singleton<EffectUIPool>
{
    [SerializeField] private TextEffect _textPrefab;
    [SerializeField] private GoldEffect _goldPrefab;

    [SerializeField] private Transform _worldCanvas;
    [SerializeField] private int _initSize = 20;
    [SerializeField] private int _maxSize = 100;

    private Dictionary<Type, IObjectPool<EffectUI>> _pools = new Dictionary<Type, IObjectPool<EffectUI>>();

    protected override void OnAwake()
    {
        base.OnAwake();

        CreatePool<TextEffect>(_textPrefab);
        CreatePool<GoldEffect>(_goldPrefab);
    }

    private void CreatePool<T>(T prefab) where T : EffectUI
    {
        Type type = typeof(T);
        if (_pools.ContainsKey(type)) return;

        var pool = new ObjectPool<EffectUI>(
            createFunc: () => Instantiate(prefab, _worldCanvas),
            actionOnGet: (obj) => obj.gameObject.SetActive(true),
            actionOnRelease: (obj) => obj.gameObject.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj.gameObject),
            collectionCheck: false,
            defaultCapacity: _initSize,
            maxSize: _maxSize
            );

        _pools.Add(type, pool);
    }

    public T Get<T>() where T : EffectUI
    {
        Type type = typeof(T);
        
        if(!_pools.ContainsKey(type)) return null;
        if (_pools[type].CountInactive > _maxSize) return null;

        return _pools[type].Get() as T;
    }

    public void Release<T>(T effect) where T : EffectUI
    {
        Type type = typeof(T);
        if (_pools.ContainsKey(type))
        {
            _pools[type].Release(effect);
        }
    }
}
