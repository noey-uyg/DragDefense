using UnityEngine;

public class Orbital : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private float _radius;

    private int _damage;
    private bool _isCritical;
    private Vector2 _startDirection;

    public Transform GetTranform { get { return _transform; } }
    public int Damage { get { return _damage; } }
    public bool IsCritical { get { return _isCritical; } }
    public float Radius { get { return _radius; } }

    public void SetDirection(Vector2 direction)
    {
        _startDirection = direction;
    }

    public void DamageSet(int damage, bool isCritical)
    {
        _damage = damage;
        _isCritical = isCritical;
    }

    public void SetPositionByAngle(float currentAngle, float radius)
    {
        Vector2 rotatedDir = Quaternion.Euler(0, 0, currentAngle) * _startDirection;
        _transform.position = rotatedDir * radius;
    }
}
