using System.Collections.Generic;
using UnityEngine;

public class BlastShot : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    [SerializeField] private float _radius;
    
    private Vector2 _direction;
    private float _moveSpeed = 5f;
    private float _rotateSpeed = 720f;
    private int _damage;
    private bool _isCritical;

    private HashSet<int> _hitMonsterId = new HashSet<int>();

    public int Damage { get { return _damage; } }
    public bool IsCritical { get { return _isCritical; } }
    public Transform GetTransform { get { return _transform; } }
    public float Radius { get { return _radius; } }

    public bool HasHit(int instanceID) => _hitMonsterId.Contains(instanceID);
    public void AddHit(int instanceID) => _hitMonsterId.Add(instanceID);

    public void Init(Vector2 startPos, Vector2 dir, int damage, bool isCri)
    {
        _transform.position = startPos;
        _direction = dir;
        _damage = damage;
        _isCritical = isCri;

        _hitMonsterId.Clear();
    }

    public void Move(float dt)
    {
        _transform.position += (Vector3)(_direction * _moveSpeed * dt);
        _transform.Rotate(0, 0, _rotateSpeed * dt);
    }
}
