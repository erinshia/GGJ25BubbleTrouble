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
    [SerializeField] private bool _isTargetingPlayer;

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
    }
}
