using System;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public struct EnemyStats
{
    public float maxHealth;
    public float currentHealth;
    public float movementSpeed;
    public float PercentageHealthOnKill;
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyStats _stats;
    [SerializeField] protected LayerMask _playerLayer;
    [SerializeField] private bool _isLongRange;
    [SerializeField] private bool _inRange;
    
    private NavMeshAgent _navMeshAgent;
    private Weapon _weapon;
    private bool _navigationActive; 
    
    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _stats.movementSpeed;
        _weapon = GetComponent<Weapon>();
    }

    // private void Start()
    // {
    //     GlobalEventHandler.Instance.OnEnemyHit += OnEnemyHit;
    // }
    //
    // private void OnDestroy()
    // {
    //     GlobalEventHandler.Instance.OnEnemyHit -= OnEnemyHit;
    // }

    private void Update()
    {
        Physics.Raycast(transform.position, (SuperDuperCC.Instance.PlayerVisuals.position + new Vector3(0, 0.1f, 0)) - transform.position, out RaycastHit hit);
        bool shootInactive = (!_inRange || (hit.transform != null && (_playerLayer.value & (1 << hit.transform.gameObject.layer)) <= 0));
        
        if (_navigationActive && shootInactive)
        {
            _navMeshAgent.destination = SuperDuperCC.Instance.PlayerVisuals.position;
        }
        else 
        {
            _navMeshAgent.destination = transform.position;
            if(!shootInactive) _weapon.Fire((SuperDuperCC.Instance.PlayerVisuals.position + new Vector3(0, 0.1f, 0)) - transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;

        if (other.CompareTag("EnemyActivation"))
        {
            _navigationActive = true;
            return;
        }
        
        if (other.CompareTag("LongRange") && !_isLongRange) return;
        _inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerLayer.value & (1 << other.transform.gameObject.layer)) <= 0) return;
        
        if (other.CompareTag("EnemyActivation"))
        {
            _navigationActive = false;
            return;
        }

        if (other.CompareTag("ShortRange") && _isLongRange)
        {
            return;
        }
        _inRange = false;
    }
    //
    // // TODO delete test method
    // [ContextMenu("Kill Enemy")]
    // private void KillEnemy()
    // {
    //     OnEnemyHit(100);
    // }
    
    public void OnEnemyHit(int damage)
    {
        _stats.currentHealth -= damage;
        if (_stats.currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    protected virtual void HandleDeath()
    {
        GlobalEventHandler.Instance.EnemyKilled(_stats.PercentageHealthOnKill);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
