using System;
using System.Collections;
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
    private Coroutine _reloadingCoroutine;

    // TODO remove this when shooting is implemented
    private void Update()
    {
        if (playerStats.currentAmmo > 0)
        {
            GetComponent<Weapon>().Fire(transform.forward);
        }
    }

    private void Start()
    {
        GlobalEventHandler.Instance.OnPlayerHit += OnPlayerHit;
        GlobalEventHandler.Instance.OnEnemyKilled += OnHealthRegained;
        GlobalEventHandler.Instance.OnAmmoUsed += OnAmmoUsed;
        GlobalEventHandler.Instance.OnReloadFinished += ReloadFinished;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnPlayerHit -= OnPlayerHit;
        GlobalEventHandler.Instance.OnEnemyKilled -= OnHealthRegained;
        GlobalEventHandler.Instance.OnAmmoUsed -= OnAmmoUsed;
        GlobalEventHandler.Instance.OnReloadFinished -= ReloadFinished;
    }

    private void OnAmmoUsed()
    {
        playerStats.currentAmmo--;
        if (playerStats.currentAmmo <= 0 && _reloadingCoroutine == null)
        {
            GlobalEventHandler.Instance.OnReload();
        }
    }

    private void ReloadFinished()
    {
        Debug.Log("Player resources reload finished");
        playerStats.currentAmmo = playerStats.maxAmmo;
        _reloadingCoroutine = null;
    }

    private void OnHealthRegained(float percentageRegained)
    {
        playerStats.currentHealth = Mathf.Clamp(playerStats.maxHealth * percentageRegained / 100f + playerStats.currentHealth, 0, playerStats.maxHealth);
    }

    private void OnPlayerHit(int damage)
    {
        playerStats.currentHealth -= damage;
        if (playerStats.currentHealth <= 0)
            GlobalEventHandler.Instance.TriggerGameOver();
    }
}
