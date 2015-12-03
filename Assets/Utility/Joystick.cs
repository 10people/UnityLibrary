using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Rein of personal player control.
/// </summary>
public class Joystick : MonoBehaviour
{
    public Camera m_CurrentCamera;

    public Transform m_JoystickBG;
    public Transform m_JoystickRein;

    private UICamera.MouseOrTouch m_MouseOrTouch;

    public Vector3 m_Offset;

    /// <summary>
    /// Max distance of radius of BG.
    /// </summary>
    public const float MaxReinDistance = 60;

    void Update()
    {
        if (m_MouseOrTouch != null)
        {
            if (!m_MouseOrTouch.pressStarted)
            {
                m_MouseOrTouch = null;
                m_JoystickBG.localPosition = Vector3.zero;
                m_JoystickRein.localPosition = Vector3.zero;
                m_Offset = Vector3.zero;

                return;
            }
            else
            {
                //put whole joystick to current finger/cursor position.
                m_JoystickRein.position = m_CurrentCamera.ScreenToWorldPoint(m_MouseOrTouch.pos);

                //restrict toggle's position within the BG.
                float Dis = Vector3.Distance(Vector3.zero, m_JoystickRein.localPosition);
                if (Dis > MaxReinDistance)
                {
                    m_JoystickRein.localPosition = m_JoystickRein.localPosition.normalized * MaxReinDistance;
                }
                m_Offset = new Vector3(m_JoystickRein.localPosition.x, 0, m_JoystickRein.localPosition.y);
            }
        }
    }
}
