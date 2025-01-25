using System;
using UnityEngine;

public class GlobalEventHandler : MonoBehaviour
{
    public static GlobalEventHandler Instance;
    public Action<int> OnPlayerHit;
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

    [ContextMenu("Test Player Health")]
    public void TestPlayerHealth()
    {
        OnPlayerHit?.Invoke(100);
    }
    
    public void PlayerHit(int damage)
    {
        OnPlayerHit?.Invoke(damage);
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
}
