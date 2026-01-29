using UnityEngine;
using UnityEngine.Pool;
public class SkillPool : Singleton<SkillPool>
{
    [SerializeField] private Transform _parent;

    [SerializeField] private BlastShot _blastPrefab;
    private ObjectPool<BlastShot> _blastPool;

    private const int MAXSIZE = 1000;
    private const int INITSIZE = 200;

    protected override void OnAwake()
    {
        _blastPool = new ObjectPool<BlastShot>(CreateGetBlastShot, ActivateGetBlastShot, DisableGetBlastShot, DestroyGetBlastShot, false, INITSIZE, MAXSIZE);
    }

    private BlastShot CreateGetBlastShot()
    {
        return Instantiate(_blastPrefab, _parent);
    }

    private void ActivateGetBlastShot(BlastShot blast)
    {
        blast.gameObject.SetActive(true);
    }

    private void DisableGetBlastShot(BlastShot blast)
    {
        blast.gameObject.SetActive(false);
    }

    private void DestroyGetBlastShot(BlastShot blast)
    {
        if (blast != null) Destroy(blast.gameObject);
    }


    public BlastShot GetBlastShot()
    {
        return _blastPool.Get();
    }

    public void ReleaseBlastShot(BlastShot blast)
    {
        _blastPool.Release(blast);
    }
}
