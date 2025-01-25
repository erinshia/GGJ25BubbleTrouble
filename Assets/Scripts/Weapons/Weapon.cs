using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [FormerlySerializedAs("Damage")] [SerializeField] private int _damage;
    [FormerlySerializedAs("Damage")] [SerializeField] private float _speed;
    [FormerlySerializedAs("Cooldown")] [SerializeField] private int _cooldown;
    [SerializeField] private float _projectileLifeTime;
    private float _timer = 0;
    [SerializeField] private float  _range;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private Projectile _projectilePrefab;
    
    // TODO remove this
    [SerializeField] protected GameObject _player;

    private void Update()
    {
        float dist = Vector3.Distance(transform.position, _player.transform.position);
        if (dist < _range)
        {
            if (_timer < _cooldown)
                _timer += Time.deltaTime;
            else
            {
                _timer = 0;
                Fire();
            }
        }
    }

    private void Fire()
    {
        Vector3 direction = _player.transform.position - transform.position;
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, _projectilePrefab.transform.rotation);
        projectile.Initialize(direction, _speed, _damage, _targetLayer, _projectileLifeTime);
    }
}
