using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SobelEdgeDetectionPass : ScriptableRenderPass
{
    private Material sobelMaterial;

    // We'll track these RTHandles instead of RenderTargetHandle
    private RTHandle cameraColorTarget;
    private RTHandle temporaryColorTexture;

    private string profilerTag;

    public SobelEdgeDetectionPass(Material mat, string tag)
    {
        sobelMaterial = mat;
        profilerTag = tag;

        // For typical final post-processing
        renderPassEvent = RenderPassEvent.AfterRendering;
    }

    // Called once before Execute, per camera
    [Obsolete(
        "This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.",
        false)]
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (sobelMaterial == null) return;

        // Let URP know we need the camera color as input
        ConfigureInput(ScriptableRenderPassInput.Color);

        // Access the built-in camera color RTHandle
        cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;

        // Create a new temporary RTHandle descriptor
        var desc = renderingData.cameraData.cameraTargetDescriptor;
        desc.depthBufferBits = 0;

        RenderingUtils.ReAllocateIfNeeded(
            ref temporaryColorTexture,
            desc,
            FilterMode.Bilinear,
            TextureWrapMode.Clamp,
            name: "_SobelTempRT"
        );

        // We also “configure” which render target(s) this pass will render into.
        // In many cases, you might do: ConfigureTarget(cameraColorTarget);
        // But if you plan to do a “source -> temp -> source” Blit, 
        // you can just rely on the Blit calls in Execute.
    }

    [Obsolete(
        "This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.",
        false)]
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (sobelMaterial == null) return;

        CommandBuffer cmd = CommandBufferPool.Get(profilerTag);

        // Blit from cameraColorTarget to our temp using the Sobel material
        Blit(cmd, cameraColorTarget, temporaryColorTexture, sobelMaterial, 0);

        // Then Blit back to cameraColorTarget
        Blit(cmd, temporaryColorTexture, cameraColorTarget);

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // Release the temporary RTHandle
        temporaryColorTexture?.Release();
    }
}