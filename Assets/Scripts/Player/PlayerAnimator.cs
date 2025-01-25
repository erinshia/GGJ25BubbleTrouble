using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Shooting = Animator.StringToHash("Shooting");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GlobalEventHandler.Instance.OnPlayerJump += PlayJumpAnimation;
    }

    private void OnDestroy()
    {
        GlobalEventHandler.Instance.OnPlayerJump -= PlayJumpAnimation;
    }

    private void PlayJumpAnimation()
    {
        _animator.SetTrigger(Jumping);
        _animator.ResetTrigger(Running);
        _animator.ResetTrigger(Dead);
        _animator.ResetTrigger(Shooting);
        _animator.ResetTrigger(Idle);
    }
    
    private void PlayRunningAnimation()
    {
        _animator.SetTrigger(Running);
        _animator.ResetTrigger(Jumping);
        _animator.ResetTrigger(Dead);
        _animator.ResetTrigger(Shooting);
        _animator.ResetTrigger(Idle);
    }
}
