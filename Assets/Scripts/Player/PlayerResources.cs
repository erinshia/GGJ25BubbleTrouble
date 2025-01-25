using System;
using UnityEngine;

[Serializable]
public struct PlayerStats
{
    public float maxHealth;
    public float currentHealth;
    public float maxAmmo;
    public float currentAmmo;
}

public class PlayerResources : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    
    private void Start()
    {
        GlobalEventHandler.Instance.OnPlayerHit += OnPlayerHit;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnPlayerHit -= OnPlayerHit;
    }

    private void OnPlayerHit(int damage)
    {
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
            GlobalEventHandler.Instance.TriggerGameOver();
    }
}
