using System;
using UnityEngine;

public class GlobalEventHandler : MonoBehaviour
{
    public static GlobalEventHandler Instance;
    public Action<int> OnPlayerHit;
    public Action<int> OnEnemyHit;
    public Action<float> OnEnemyKilled;
    public Action OnAmmoUsed;
    public Action OnGameOver;
    public Action OnWin;
    public Action OnReload;
    public Action OnReloadFinished;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    [ContextMenu("Trigger Game Over")]
    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }
    
    [ContextMenu("Trigger Win")]
    public void TriggerWin()
    {
        OnWin?.Invoke();
    }
    
    [ContextMenu("Trigger Reload")]
    public void TriggerReload()
    {
        OnReload?.Invoke();
    }
    
    [ContextMenu("Trigger Reload")]
    public void ReloadFinished()
    {
        OnReloadFinished?.Invoke();
    }
    
    [ContextMenu("Test Player Health")]
    public void TestPlayerHealth()
    {
        OnPlayerHit?.Invoke(10);
    }

    public void EnemyHit(int damage)
    {
        OnEnemyHit?.Invoke(damage);
    }
    
    public void PlayerHit(int damage)
    {
        OnPlayerHit?.Invoke(damage);
    }

    public void EnemyKilled(float precentageRestored)
    {
        OnEnemyKilled?.Invoke(precentageRestored);
    }

    public void AmmoUsed()
    {
        OnAmmoUsed?.Invoke();
    }
}
