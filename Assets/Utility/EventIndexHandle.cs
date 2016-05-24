using UnityEngine;
using System.Collections;

public class EventIndexHandle : MonoBehaviour
{
    public delegate void TouchedSend(int index);
    public event TouchedSend m_Handle;
    public int m_SendIndex;
    public bool m_isDrag = false;

    public bool IsMultiClickCheck = true;
    public float MultiClickDuration = 0.2f;
    private float lastClickTime;

    public void OnClick()
    {
        //Disable the next click if click too quickly.
        if (IsMultiClickCheck)
        {
            if (Time.realtimeSinceStartup - lastClickTime < MultiClickDuration)
            {
                return;
            }

            lastClickTime = Time.realtimeSinceStartup;
        }

        if (m_Handle != null)
            m_Handle(m_SendIndex);
    }
}