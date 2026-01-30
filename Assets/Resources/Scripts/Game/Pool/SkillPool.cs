using UnityEngine;
using UnityEngine.Pool;
public class SkillPool : Singleton<SkillPool>
{
    [SerializeField] private Transform _parent;

    [SerializeField] private BlastShot _blastPrefab;
    [SerializeField] private Orbital _orbitalPrefab;
    
    private ObjectPool<BlastShot> _blastPool;
    private ObjectPool<Orbital> _orbitalPool;

    private const int MAXSIZE = 1000;
    private const int INITSIZE = 200;

    protected override void OnAwake()
    {
        _blastPool = new ObjectPool<BlastShot>(CreateGetBlastShot, ActivateGetBlastShot, DisableGetBlastShot, DestroyGetBlastShot, false, INITSIZE, MAXSIZE);
        _orbitalPool = new ObjectPool<Orbital>(CreateGetOrbital, ActivateGetOrbital, DisableGetOrbital, DestroyGetOrbital, false, INITSIZE, MAXSIZE);
    }

    #region Blast
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
    #endregion

    #region Orbital
    private Orbital CreateGetOrbital()
    {
        return Instantiate(_orbitalPrefab, _parent);
    }

    private void ActivateGetOrbital(Orbital orbital)
    {
        orbital.gameObject.SetActive(true);
    }

    private void DisableGetOrbital(Orbital orbital)
    {
        orbital.gameObject.SetActive(false);
    }

    private void DestroyGetOrbital(Orbital orbital)
    {
        if (orbital != null) Destroy(orbital.gameObject);
    }

    public Orbital GetOrbital()
    {
        return _orbitalPool.Get();
    }

    public void ReleaseOrbital(Orbital orbital)
    {
        _orbitalPool.Release(orbital);
    }
    #endregion
}
