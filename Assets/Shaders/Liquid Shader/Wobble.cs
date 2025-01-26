using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class Wobble : MonoBehaviour
{
    private Renderer _renderer;
    private Vector3 _lastPos;
    private Vector3 _velocity;
    private Vector3 _lastRot;
    private Vector3 _angularVelocity;
    public float MaxWobble = 0.03f;
    public float WobbleSpeed = 1f;
    public float Recovery = 1f;
    [FormerlySerializedAs("glassBottom")] public Transform GlassBottom;
    
    private float _wobbleAmountX;
    private float _wobbleAmountY; 
    private float _wobbleAmountZ;
    private float _wobbleAmountToAddX;
    private float _wobbleAmountToAddY;
    private float _wobbleAmountToAddZ;
    private float _pulse;
    private float _time = 0.5f;
    private readonly int _shaderPropertyWobbleX = Shader.PropertyToID("_WobbleX");
    private readonly int _shaderPropertyWobbleY = Shader.PropertyToID("_WobbleY");
    private readonly int _shaderPropertyWobbleZ = Shader.PropertyToID("_WobbleZ");
    private readonly int _shaderPropertyGlassBottom = Shader.PropertyToID("_Glass_Bottom");

    // Use this for initialization
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        _time += Time.deltaTime;
        float recoveryStep = Time.deltaTime * Recovery;
        // decrease wobble over time
        _wobbleAmountToAddX = Mathf.Lerp(_wobbleAmountToAddX, 0, recoveryStep);
        _wobbleAmountToAddY = Mathf.Lerp(_wobbleAmountToAddY, 0, recoveryStep);
        _wobbleAmountToAddZ = Mathf.Lerp(_wobbleAmountToAddZ, 0, recoveryStep);

        // make a sine wave of the decreasing wobble
        _pulse = 2 * Mathf.PI * WobbleSpeed;
        float pulseFractionSine = Mathf.Sin(_pulse * _time);
        _wobbleAmountX = _wobbleAmountToAddX * pulseFractionSine;
        _wobbleAmountY = _wobbleAmountToAddY * pulseFractionSine;
        _wobbleAmountZ = _wobbleAmountToAddZ * pulseFractionSine;

        // send it to the shader
        if(Application.isEditor && !Application.isPlaying)
        {
            _renderer.sharedMaterial.SetFloat(_shaderPropertyWobbleX, _wobbleAmountX);
            _renderer.sharedMaterial.SetFloat(_shaderPropertyWobbleY, _wobbleAmountY);
            _renderer.sharedMaterial.SetFloat(_shaderPropertyWobbleZ, _wobbleAmountZ);
            _renderer.sharedMaterial.SetVector(_shaderPropertyGlassBottom, GlassBottom.position);
        }
        else
        {
            _renderer.material.SetFloat(_shaderPropertyWobbleX, _wobbleAmountX);
            _renderer.material.SetFloat(_shaderPropertyWobbleY, _wobbleAmountY);
            _renderer.material.SetFloat(_shaderPropertyWobbleZ, _wobbleAmountZ);
            _renderer.material.SetVector(_shaderPropertyGlassBottom, GlassBottom.position);
        }
        

        // velocity
        _velocity = (_lastPos - transform.position) / Time.deltaTime;
        _angularVelocity = transform.rotation.eulerAngles - _lastRot;

        // add clamped velocity to wobble
        _wobbleAmountToAddX += Mathf.Clamp((_velocity.x + (_angularVelocity.x * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        _wobbleAmountToAddY += Mathf.Clamp((_velocity.y + (_angularVelocity.y * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);
        _wobbleAmountToAddZ += Mathf.Clamp((_velocity.z + (_angularVelocity.z * 0.2f)) * MaxWobble, -MaxWobble, MaxWobble);

        // keep last position
        _lastPos = transform.position;
        _lastRot = transform.rotation.eulerAngles;
    }
}