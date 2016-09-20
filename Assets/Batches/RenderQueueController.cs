using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RenderQueueController : MonoBehaviour
{
    public int RenderQueue = -1;

    private List<ParticleSystem> particleSystemList;
    private List<MeshRenderer> meshRendererList;
    private List<SkinnedMeshRenderer> skinnedMeshRendererList;
    private Material material;

    public void GetRenderQueue()
    {
        if (material != null)
        {
            Debug.Log("Update render queue.");
            RenderQueue = material.renderQueue;
        }
        else
        {
            Debug.LogError("No material.");
        }
    }

    private void GetMaterial()
    {
        var sprite = GetComponent<UISprite>();
        if (sprite != null)
        {
            material = sprite.material;
            return;
        }

        var skin = GetComponentInChildren<SkinnedMeshRenderer>();
        if (skin != null)
        {
            material = skin.material;
            return;
        }

        Debug.LogError("Cannot get material.");
    }

    private float lastUpdateTime;

    void Update()
    {
        if (Time.realtimeSinceStartup - lastUpdateTime > 1f)
        {
            GetRenderQueue();
        }
    }

    void Start()
    {
        GetMaterial();
    }
}
