using UnityEngine;

public class SobelEdgePostProcess : MonoBehaviour
{
    public Material EdgeMaterial;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, EdgeMaterial);
    }
}