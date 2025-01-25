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
        GlobalEventHandler.Instance.OnHealthRegained += OnHealthRegained;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnPlayerHit -= OnPlayerHit;
        GlobalEventHandler.Instance.OnHealthRegained -= OnHealthRegained;
    }

    private void OnHealthRegained(int percentageRegained)
    {
        playerStats.currentHealth = Mathf.Clamp(playerStats.maxHealth * percentageRegained + playerStats.currentHealth, 0, playerStats.maxHealth);
    }

    private void OnPlayerHit(int damage)
    {
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
            GlobalEventHandler.Instance.TriggerGameOver();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollisionEnter Player " + other.gameObject.name);
    }
}
