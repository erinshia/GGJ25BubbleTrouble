using UnityEngine;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private float _speed;
    [SerializeField] private float _projectileLifeTime;
    [SerializeField] private float  _range;
    [SerializeField] private int _damage;
    [SerializeField] private int _cooldown;
    [SerializeField] private bool _isTargetingPlayer;

    private float _timer;
    
    private void Update()
    {
        if (_timer > 0)
            _timer -= Time.deltaTime;
    }

    public void Fire(Vector3 direction)
    {
        if (_timer > 0) return;
        Projectile projectile = Instantiate(_projectilePrefab, transform.position, _projectilePrefab.transform.rotation);
        projectile.Initialize(direction, _speed, _damage, _targetLayer, _projectileLifeTime, _isTargetingPlayer);
        _timer = _cooldown;
        if(!_isTargetingPlayer)
            GlobalEventHandler.Instance.AmmoUsed();
    }
}
