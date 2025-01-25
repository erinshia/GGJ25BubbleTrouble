using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private int _damage;
    private float _speed;
    private Vector3 _direction;
    private float _lifetime = 5;
    private float timer;
    [SerializeField] private LayerMask _targetLayer;
    
    public void Initialize(Vector3 direction, float speed, int damage, LayerMask targetLayer, float lifetime)
    {
        _damage = damage;
        _targetLayer = targetLayer;
        _speed = speed;
        _lifetime = lifetime;
        GetComponent<Rigidbody>().AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    private void Update()
    {
        //transform.position += _direction * (_speed * Time.deltaTime);
        //Raycast
        timer += Time.deltaTime;
        if(timer >= _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_targetLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        GlobalEventHandler.Instance.OnPlayerHit(_damage);
        gameObject.SetActive(false);
        Destroy(this);
    }
}
