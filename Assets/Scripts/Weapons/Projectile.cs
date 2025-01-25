using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int _damage;
    private Vector3 _direction;
    private float _lifetime = 5;
    private float timer;
    [SerializeField] private LayerMask _targetLayer;
    private bool _targetIsPlayer;
    
    public void Initialize(Vector3 direction, float speed, int damage, LayerMask targetLayer, float lifetime,bool targetIsPlayer)
    {
        _damage = damage;
        _targetLayer = targetLayer; 
        _lifetime = lifetime;
        _targetIsPlayer = targetIsPlayer;
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
