using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

public class EffectPool : Singleton<EffectPool>
{
    [System.Serializable]
    public class EffectEntry
    {
        public EffectType type;
        public ParticleSystem prefab;
        public int initSize;
        public int maxSize;
    }

    [SerializeField] private List<EffectEntry> _effectEntries;

    private Dictionary<EffectType, IObjectPool<ParticleSystem>> _pools = new Dictionary<EffectType, IObjectPool<ParticleSystem>>();

    protected override void OnAwake()
    {
        base.OnAwake();

        foreach(var v in _effectEntries)
        {
            var cur = v;

            var pool = new ObjectPool<ParticleSystem>(
                createFunc: () => CreateEffect(cur.prefab),
                actionOnGet: ActivateEffect,
                actionOnRelease: DisableEffect,
                actionOnDestroy: DestroyEffect,
                collectionCheck: false,
                defaultCapacity: cur.initSize,
                maxSize: cur.maxSize
                );

            _pools.Add(cur.type, pool);
        }
    }

    private ParticleSystem CreateEffect(ParticleSystem ps)
    {
        return Instantiate(ps);
    }

    private void ActivateEffect(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
    }

    private void DisableEffect(ParticleSystem ps)
    {
        ps.gameObject.SetActive(false);
    }

    private void DestroyEffect(ParticleSystem ps)
    {
        if(ps!=null) Destroy(ps.gameObject);
    }

    public ParticleSystem GetEffect(EffectType type)
    {
        if(!_pools.ContainsKey(type)) return null;

        var pool = _pools[type];
        var entry = _effectEntries.Find(x => x.type == type);

        if (pool.CountInactive > entry.maxSize) return null;

        return pool.Get();
    }

    public void ReleaseEffect(EffectType type, ParticleSystem ps)
    {
        if (ps == null) return;

        if (_pools.ContainsKey(type))
        {
            _pools[type].Release(ps);
        }
        else
        {
            Destroy(ps.gameObject);
        }
    }
}
