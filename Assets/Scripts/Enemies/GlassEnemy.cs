using UnityEngine;

public class GlassEnemy : Enemy
{
    private bool _inRange;

    private void Update()
    {
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out RaycastHit hit);
        
        if (!_inRange || ((_playerLayer.value & (1 << hit.transform.gameObject.layer)) <= 0))
        {
            _navMeshAgent.destination = _player.transform.position;
        }
        else
        {
            _navMeshAgent.destination = transform.position;
            _weapon.Fire(_player.transform.position - transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        
        _inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        _inRange = false;
    }
}
