using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PixelatedImageEffect : MonoBehaviour
{
    public Material effectMaterial;
    void OnRenderImage(Texture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, effectMaterial);
    }
}
