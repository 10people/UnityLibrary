//#define UNIT_TEST

using UnityEngine;
using System.Collections;

public class EffectNumController : Singleton<EffectNumController>
{
    //TODO: set ur own.
    private float m_limitDuration = 2.0f;
    //TODO: set ur own.
    private int m_limitNumber = 3;

    private float m_lastCheckTime = -1f;

    private int m_playedEffectCount;

    public bool IsCanPlayEffect()
    {
        return m_playedEffectCount < m_limitNumber;
    }

    public void NotifyPlayingEffect()
    {
        m_playedEffectCount++;
    }

    private void ResetPlayedEffectCount()
    {
        m_playedEffectCount = 0;
    }

    void Update()
    {
        if (Time.realtimeSinceStartup - m_lastCheckTime > m_limitDuration)
        {
            ResetPlayedEffectCount();
            m_lastCheckTime = Time.realtimeSinceStartup;
        }
    }

#if UNIT_TEST
    void OnGUI()
    {
        if (GUILayout.Button("Test effect num"))
        {
            if (IsCanPlayEffect())
            {
                Debug.LogWarning("==========Play effect");

                NotifyPlayingEffect();
            }
        }
    }
#endif
}
