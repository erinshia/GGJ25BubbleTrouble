using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int Jumping = Animator.StringToHash("Jumping");
    private static readonly int Running = Animator.StringToHash("Running");
    private static readonly int Dead = Animator.StringToHash("Dead");
    private static readonly int Shooting = Animator.StringToHash("Shooting");
    private static readonly int Idle = Animator.StringToHash("Idle");
    private Animator _animator;
    [SerializeField] private PlayerInput playerInput;
    private bool _isRunning;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        SuperDuperCC.Instance.OnJump += PlayJumpAnimation;
        SuperDuperCC.Instance.OnShoot += PlayShootingAnimation;
        // GlobalEventHandler.Instance.OnPlayerJump += PlayJumpAnimation;
        GlobalEventHandler.Instance.OnGameOver += PlayDeadAnimation;
    }

    private void OnDestroy()
    {
        SuperDuperCC.Instance.OnJump -= PlayJumpAnimation;
        SuperDuperCC.Instance.OnShoot -= PlayShootingAnimation;
        GlobalEventHandler.Instance.OnGameOver -= PlayDeadAnimation;
    }

    private void PlayIdleAnimation()
    {
        _animator.SetBool("isRunning", false);
        _animator.SetTrigger(Idle);
        _animator.ResetTrigger(Running);
        _animator.ResetTrigger(Dead);
        _animator.ResetTrigger(Shooting);
        _animator.ResetTrigger(Jumping);
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
        _animator.SetBool("isRunning", true);
        _animator.SetTrigger(Running);
        _animator.ResetTrigger(Jumping);
        _animator.ResetTrigger(Dead);
        _animator.ResetTrigger(Shooting);
        _animator.ResetTrigger(Idle);
    }

    private void PlayDeadAnimation()
    {
        _animator.SetTrigger(Dead);
        _animator.ResetTrigger(Running);
        _animator.ResetTrigger(Idle);
        _animator.ResetTrigger(Shooting);
        _animator.ResetTrigger(Jumping);
    }
    
    //TODO change this to use the params
    private void PlayShootingAnimation(Vector3 arg1, Vector3 arg2)
    {
        Debug.Log("play shooting anim");
        _animator.SetTrigger(Shooting);
        _animator.ResetTrigger(Running);
        _animator.ResetTrigger(Idle);
        _animator.ResetTrigger(Dead);
        _animator.ResetTrigger(Jumping);
    }
}
