using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapBeenAttackEffectController : MonoBehaviour
{
    public MapController m_MapController;

    public GameObject CircleEffectObject;
    public GameObject QuadEffectObject;

    public GameObject BeenAttackEffectPrefab;

    public static float EffectDuration = 5.0f;
    public static float MapEffectChangeDuration = 0.2f;
    public static float BeenAttackPeriodDuration = 0.5f;

    public static List<int> BeenAttackEffectUidList = new List<int>();

    public void BlinkEffect(int p_uid, Vector3 p_beenAttackEffectPos)
    {
        if (!BeenAttackEffectUidList.Contains(p_uid))
        {
            BeenAttackEffectUidList.Add(p_uid);

            if (m_MapController.m_IsMapInSmallMode)
            {
                BlinkCircleEffect();
            }
            else
            {
                BlinkQuadEffect();
            }

            BlinkBeenAttackEffect(p_uid, p_beenAttackEffectPos);
        }
    }

    private void BlinkCircleEffect()
    {
        if (Utils.Instance.IsTimeCalcKeyExist("MapCircleEffect"))
        {
            Utils.Instance.RemoveFromTimeCalc("MapCircleEffect");
        }
        Utils.Instance.AddFrameDelegateToTimeCalc("MapCircleEffect", EffectDuration, SetCircleEffectBlinkState);
    }

    private void SetCircleEffectBlinkState(float elapseTime)
    {
        if (EffectDuration - elapseTime <= 0)
        {
            Utils.Instance.RemoveFromTimeCalc("MapCircleEffect");
            CircleEffectObject.SetActive(false);
        }
        else
        {
            CircleEffectObject.SetActive((EffectDuration - elapseTime) / MapEffectChangeDuration % 2 > 1);
        }
    }

    private void BlinkQuadEffect()
    {
        if (Utils.Instance.IsTimeCalcKeyExist("MapQuadEffect"))
        {
            Utils.Instance.RemoveFromTimeCalc("MapQuadEffect");
        }
        Utils.Instance.AddFrameDelegateToTimeCalc("MapQuadEffect", EffectDuration, SetQuadEffectBlinkState);
    }

    private void SetQuadEffectBlinkState(float elapseTime)
    {
        if (EffectDuration - elapseTime <= 0)
        {
            Utils.Instance.RemoveFromTimeCalc("MapQuadEffect");
            QuadEffectObject.SetActive(false);
        }
        else
        {
            QuadEffectObject.SetActive((EffectDuration - elapseTime) / MapEffectChangeDuration % 2 > 1);
        }
    }

    public void BlinkBeenAttackEffect(int p_uid, Vector3 p_beenAttackEffectPos)
    {
        var go = Instantiate(BeenAttackEffectPrefab);
        Utils.ActiveWithStandardize(transform, go.transform);
        go.transform.localPosition = p_beenAttackEffectPos;
        go.name += "_" + p_uid;

        var controller = go.GetComponent<MapBeenAttackEffectItem>();
        controller.m_BeenAttackUid = p_uid;
        controller.ShowEffect();
    }
}
