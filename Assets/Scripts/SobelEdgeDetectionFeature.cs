using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class SobelEdgeDetectionFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class SobelSettings
    {
        public Material sobelMaterial;
    }

    public SobelSettings settings = new();

    private SobelEdgeDetectionPass sobelPass;

    public override void Create()
    {
        if (!settings.sobelMaterial)
        {
            Debug.LogWarning("No Sobel Material assigned.");
            return;
        }

        sobelPass = new SobelEdgeDetectionPass(settings.sobelMaterial, "SobelEdgeDetection");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.sobelMaterial) return;
        renderer.EnqueuePass(sobelPass);
    }
}