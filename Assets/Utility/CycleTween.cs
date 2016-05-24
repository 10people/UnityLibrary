using UnityEngine;
using System.Collections;
using System.Linq;

public class CycleTween : MonoBehaviour
{
    public struct CycleTweenConfig
    {
        public float m_StartValue;
        public float m_EndValue;
        public float m_Duration;
        public DelegateUtil.FloatDelegate m_FloatDelegate;
    }

    private CycleTweenConfig m_cycleTweenConfig;

    public static void StartCycleTween(GameObject targetGameObject, float startValue, float endValue, float duration, DelegateUtil.FloatDelegate floatDelegate)
    {
        var cycleTween = targetGameObject.GetComponent<CycleTween>() ?? targetGameObject.AddComponent<CycleTween>();

        cycleTween.m_cycleTweenConfig = new CycleTweenConfig()
        {
            m_StartValue = startValue,
            m_EndValue = endValue,
            m_Duration = duration,
            m_FloatDelegate = floatDelegate
        };

        cycleTween.DoCycleTween();
    }

    private void DoCycleTween()
    {
        gameObject.GetComponentsInChildren<iTween>().ToList().ForEach(item => Destroy(item));

        iTween.ValueTo(gameObject, iTween.Hash("from", m_cycleTweenConfig.m_StartValue, "to", m_cycleTweenConfig.m_EndValue, "time", m_cycleTweenConfig.m_Duration, "easetype", "easeInOutSine", "onupdate", "OnUpdateCycleValue", "oncomplete", "OnEndCycleValue"));
    }

    private void OnUpdateCycleValue(float value)
    {
        m_cycleTweenConfig.m_FloatDelegate(value);
    }

    private void OnEndCycleValue()
    {
        if (gameObject == null || !gameObject.activeInHierarchy)
        {
            return;
        }

        Utils.SwapValue(ref m_cycleTweenConfig.m_StartValue, ref m_cycleTweenConfig.m_EndValue);
        DoCycleTween();
    }
}
