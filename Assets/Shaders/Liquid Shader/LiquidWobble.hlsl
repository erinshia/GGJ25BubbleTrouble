// Custom Function: WobbleCalculation
// Parameters:
// - float recovery
// - float wobbleSpeed
// - float maxWobble
// - float3 objectToWorld[3] (object space to world space transformation matrix)
// - float3 vertexPosition (in object space)
// - float3 normal
// - float3 lastObjectPosition (in object space)

void LiquidWobble_float(float recovery, float wobbleSpeed, float maxWobble, float3 vertexPosition, float3 normal,
                        float3 lastObjectPosition, out float wobbleAmountX, out float wobbleAmountZ)
{
    const float pulse = 6.28319 * wobbleSpeed; // 2 * PI

    wobbleAmountX = lerp(0, 0, recovery * _Time.x);
    wobbleAmountZ = lerp(0, 0, recovery * _Time.x);

    wobbleAmountX += clamp((normal.x + (normal.z * 0.2)) * maxWobble, -maxWobble, maxWobble);
    wobbleAmountZ += clamp((normal.z + (normal.x * 0.2)) * maxWobble, -maxWobble, maxWobble);

    // Transform vertex position to world space
    float3 worldVertexPosition = mul(unity_ObjectToWorld, float4(vertexPosition, 1.0)).xyz;

    // Check if the object has moved
    float3 velocity = (worldVertexPosition - lastObjectPosition) / _Time.x;

    // Use world space position and velocity to approximate object's movement
    wobbleAmountX *= sin(pulse * (worldVertexPosition.x + _Time.x + velocity.x));
    wobbleAmountZ *= sin(pulse * (worldVertexPosition.z + _Time.x + velocity.z));

    // return float2(wobbleAmountX, wobbleAmountZ);
}
