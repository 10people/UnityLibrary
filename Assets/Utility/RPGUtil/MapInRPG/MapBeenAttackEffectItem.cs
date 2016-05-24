using System;
using UnityEngine;

public class MapBeenAttackEffectItem : MonoBehaviour
{
    public int m_BeenAttackUid = -1;

    public GameObject BeenAttackEffect1;
    public GameObject BeenAttackEffect2;

    public void ShowEffect()
    {
        BeenAttackEffect2.SetActive(true);

        if (Utils.Instance.IsTimeCalcKeyExist("BeenAttackEffect" + m_BeenAttackUid))
        {
            Utils.Instance.RemoveFromTimeCalc("BeenAttackEffect" + m_BeenAttackUid);
        }
        Utils.Instance.AddFrameDelegateToTimeCalc("BeenAttackEffect" + m_BeenAttackUid, MapBeenAttackEffectController.EffectDuration, SetEffectState);
    }

    private void SetEffectState(float elapseTime)
    {
        if (MapBeenAttackEffectController.EffectDuration - elapseTime <= 0)
        {
            if (MapBeenAttackEffectController.BeenAttackEffectUidList.Contains(m_BeenAttackUid))
            {
                MapBeenAttackEffectController.BeenAttackEffectUidList.Remove(m_BeenAttackUid);
            }

            Utils.Instance.RemoveFromTimeCalc("BeenAttackEffect" + m_BeenAttackUid);
            BeenAttackEffect2.SetActive(false);

            Destroy(gameObject);
        }
        else
        {
            var flag = elapseTime % MapBeenAttackEffectController.BeenAttackPeriodDuration / MapBeenAttackEffectController.BeenAttackPeriodDuration;
            BeenAttackEffect2.transform.localScale = Vector3.one * (1 - flag);
        }
    }
}
