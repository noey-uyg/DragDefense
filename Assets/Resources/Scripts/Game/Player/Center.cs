using UnityEngine;

public class Center : MonoBehaviour
{
    private float _currentHP;
    private float _defense;

    public void Init()
    {
        _currentHP = PlayerStat.CurMaxHP;
        _defense = PlayerStat.CurDamageReduction;
    }
}
