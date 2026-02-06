using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.Pool;

public class EffectUIPool : Singleton<EffectUIPool>
{
    [SerializeField] private TextEffect _textPrefab;
    [SerializeField] private GoldEffect _goldPrefab;

    [SerializeField] private Transform _textContainer;
    [SerializeField] private Transform _imageContainer;
    [SerializeField] private int _initSize = 20;
    [SerializeField] private int _maxSize = 100;

    private Dictionary<Type, IObjectPool<EffectUI>> _pools = new Dictionary<Type, IObjectPool<EffectUI>>();

    protected override void OnAwake()
    {
        base.OnAwake();

        CreatePool<TextEffect>(_textPrefab, _textContainer);
        CreatePool<GoldEffect>(_goldPrefab, _imageContainer);
    }

    private void CreatePool<T>(T prefab, Transform container) where T : EffectUI
    {
        Type type = typeof(T);
        if (_pools.ContainsKey(type)) return;

        var pool = new ObjectPool<EffectUI>(
            createFunc: () => Instantiate(prefab, container),
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

        var pool = _pools[type] as ObjectPool<EffectUI>;
        if (pool != null && pool.CountAll > _maxSize) return null;

        return pool.Get() as T;
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
