using UnityEngine;

public class BottleEnemy : Enemy
{
    private void Update()
    {
        Physics.Raycast(transform.position, _player.transform.position - transform.position, out RaycastHit hit);
        
        if ((_playerLayer.value & (1 << hit.transform.gameObject.layer)) > 0)
        {
            _navMeshAgent.destination = transform.position;
            _weapon.Fire(_player.transform.position - transform.position);
        }
    }
}
