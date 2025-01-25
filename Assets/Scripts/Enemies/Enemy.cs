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
    // TODO remove this
    [SerializeField] protected GameObject _player;
    protected NavMeshAgent _navMeshAgent;
    [SerializeField] protected LayerMask _playerLayer;
    protected Weapon _weapon;
    
    protected virtual void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _weapon = GetComponent<Weapon>();
    }

    private void Start()
    {
        GlobalEventHandler.Instance.OnEnemyHit += OnEnemyHit;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnEnemyHit -= OnEnemyHit;
    }

    [ContextMenu("Kill Enemy")]
    private void KillEnemy()
    {
        OnEnemyHit(10000);
    }
    
    private void OnEnemyHit(int damage)
    {
        _stats.currentHealth -= damage;
        if (_stats.currentHealth <= 0)
        {
            GlobalEventHandler.Instance.EnemyKilled(_stats.PercentageHealthOnKill);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
