using UnityEngine;

public class GlassEnemy : Enemy
{
    [SerializeField] protected LayerMask _playerLayer;
    private bool _inRange = false;
    private void Update()
    {
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out RaycastHit hit);
        Debug.Log("raycast hit: " + hit.transform.gameObject.name);
        
        if (!_inRange || ((_playerLayer.value & (1 << hit.transform.gameObject.layer)) <= 0))
        {
            Debug.Log("moving to player");
            _navMeshAgent.destination = _player.transform.position;
        }
        else
        {
            _navMeshAgent.destination = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enemy trigger enter");
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        
        Debug.Log("Enemy in range");
        _inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Enemy trigger exit");
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        Debug.Log("Enemy not in range");
        _inRange = false;
    }
}
