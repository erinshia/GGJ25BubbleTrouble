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
    
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
}
