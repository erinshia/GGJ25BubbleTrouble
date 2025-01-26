using UnityEngine;

public class Projectile : MonoBehaviour
{
    private LayerMask _targetLayer;
    protected Vector3 _direction;
    protected float _speed;
    private float _lifetime = 5;
    private float timer;
    private int _damage;
    private bool _targetIsPlayer;
    
    public void Initialize(Vector3 direction, float speed, int damage, LayerMask targetLayer, float lifetime,bool targetIsPlayer)
    {
        _damage = damage;
        _targetLayer = targetLayer; 
        _lifetime = lifetime;
        _targetIsPlayer = targetIsPlayer;
        _direction = direction;
        _speed = speed;
        SendFlying(direction, speed);
    }

    protected virtual void SendFlying(Vector3 direction, float speed)
    {
        GetComponent<Rigidbody>().AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_targetLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        if (_targetIsPlayer)
        {
            GlobalEventHandler.Instance.OnPlayerHit(_damage);
        }
        else
        {
            GlobalEventHandler.Instance.OnEnemyHit(_damage);
        }
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
