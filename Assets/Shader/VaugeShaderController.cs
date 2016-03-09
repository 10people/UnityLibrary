using UnityEngine;
using System.Collections;

public class VaugeShaderController : MonoBehaviour
{

    private Shader targetShader;
    private Material targetMaterial;

    // Use this for initialization
    void Awake()
    {
        targetShader = Shader.Find("Hidden/VagueShader");
        targetMaterial = new Material(targetShader);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

        targetMaterial.SetVector("_offset_xy", new Vector4(1, 0, 0, 0));
        Graphics.Blit(source, temp, targetMaterial);

        targetMaterial.SetVector("_offset_xy", new Vector4(0, 1, 0, 0));
        Graphics.Blit(temp, dest, targetMaterial);

        RenderTexture.ReleaseTemporary(temp);
    }
}
