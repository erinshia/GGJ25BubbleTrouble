using System;
using UnityEngine;

public class GlobalEventHandler : MonoBehaviour
{
    public static GlobalEventHandler Instance;
    public Action<int> OnPlayerHit;
    public Action<int> OnHealthRegained;
    public Action OnGameOver;
    public Action OnWin;

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
    
    public void RegainHealth(int percentageRegained)
    {
        OnHealthRegained?.Invoke(percentageRegained);
    }
    
    [ContextMenu("Test Player Health")]
    public void TestPlayerHealth()
    {
        OnPlayerHit?.Invoke(10);
    }
    
    public void PlayerHit(int damage)
    {
        OnPlayerHit?.Invoke(damage);
    }

}
