using UnityEngine;
using System.Collections;

public class CameraVagueEffect : MonoBehaviour
{
    public Camera m_Camera;
    public RenderTexture m_RenderTexture;

    void LateUpdate()
    {
        Graphics.DrawTexture(new Rect(10, 10, 100, 100), m_RenderTexture);
    }

    // Use this for initialization
    void Start()
    {
        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
        m_Camera.targetTexture = m_RenderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
