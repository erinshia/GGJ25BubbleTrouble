using System;
using UnityEngine;

[Serializable]
public struct EnemyStats
{
    public float maxHealth;
    public float currentHealth;
    public float damage;
    public float movementSpeed;
    public float PercentageHealthOnKill;
}

public abstract class Enemy : MonoBehaviour
{
   [SerializeField] protected EnemyStats _stats;
}
