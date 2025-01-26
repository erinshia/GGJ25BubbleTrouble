using System;
using UnityEngine;

public class GlobalEventHandler : MonoBehaviour
{
    public static GlobalEventHandler Instance;
    
    public Action<int> OnPlayerHit;
    public Action<int> OnEnemyHit;
    public Action<float> OnEnemyKilled;
    
    public Action OnGameOver;
    public Action OnWin;
    public Action OnReload;
    public Action OnReloadFinished;
    public Action OnAmmoUsed;

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
    
    public void TriggerGameOver()
    {
        OnGameOver?.Invoke();
    }
    
    public void TriggerWin()
    {
        OnWin?.Invoke();
    }
    
    public void TriggerReload()
    {
        OnReload?.Invoke();
    }
    
    public void ReloadFinished()
    {
        OnReloadFinished?.Invoke();
    }
    
    public void AmmoUsed()
    {
        OnAmmoUsed?.Invoke();
    }
}
