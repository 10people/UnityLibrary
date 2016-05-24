using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BannerEffectController : MonoBehaviour
{
    public DelegateUtil.VoidDelegate m_ExecuteAfterClick;
    public DelegateUtil.VoidDelegate m_ExecuteAfterEnd;

    public bool isAlertInfoEffectShowing = false;
    private bool m_isCanClickEffect = false;

    public GameObject AlertEffectMainObject;
    public GameObject AlertEffectBGObject;
    public GameObject AlertEffectInfoObject;
    public UILabel AlertEffectLabel;
    public UILabel AlertEffectSubLabel;
    public UILabel AlertEffectButtomInfoLabel;
    public UILabel AlertEffectRightInfoLabel;
    public UIGrid AlertEffectRewardGrid;

    private List<int> m_rewardIDs = new List<int>();
    private List<int> m_rewardNums = new List<int>();

    private const float BGTurnDuration = 0.3f;
    private const float LabelMoveDuration = 0.4f;

    /// <summary>
    /// TODO: set total width in coordinate
    /// </summary>
    private float m_TotalWidthInCoordinate;

    public void OnAlertEffectClick()
    {
        if (m_isCanClickEffect)
        {
            DenableAlertEffectClick();

            HideAlertEffect();

            if (m_ExecuteAfterClick != null)
            {
                m_ExecuteAfterClick();
            }
        }
    }

    public void ShowAlertInfo(string p_mainStr, string p_subStr = "", string p_rightStr = "", List<int> p_rewardIDs = null, List<int> p_rewardNums = null)
    {
        isAlertInfoEffectShowing = true;

        AlertEffectLabel.text = p_mainStr;
        AlertEffectSubLabel.text = p_subStr;
        AlertEffectRightInfoLabel.text = p_rightStr;

        if (p_rewardIDs != null && p_rewardIDs.Any() && p_rewardNums != null && p_rewardNums.Any() && p_rewardIDs.Count == p_rewardNums.Count)
        {
            m_rewardIDs = p_rewardIDs;
            m_rewardNums = p_rewardNums;

            //TODO: implement reward icons.

            ShowAlertEffect();
        }
        else
        {
            //Clear all awards.

            m_rewardIDs.Clear();
            m_rewardNums.Clear();

            while (AlertEffectRewardGrid.transform.childCount > 0)
            {
                var child = AlertEffectRewardGrid.transform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }

            ShowAlertEffect();
        }
    }

    public void ShowAlertEffect()
    {
        AlertEffectMainObject.SetActive(true);

        ShowAlertEffectBG();
    }

    private void ShowAlertEffectBG()
    {
        AlertEffectBGObject.transform.localScale = new Vector3(1, 0, 1);
        AlertEffectBGObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", 1, "time", BGTurnDuration, "easetype", "easeOutBack", "onupdate", "SetAlertEffectBGScale", "oncomplete", "ShowAlertEffectInfo"));
    }

    private void ShowAlertEffectInfo()
    {
        AlertEffectInfoObject.transform.localPosition = new Vector3(-m_TotalWidthInCoordinate, AlertEffectInfoObject.transform.localPosition.y, 0);
        AlertEffectInfoObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash("from", -m_TotalWidthInCoordinate, "to", 0, "time", LabelMoveDuration, "easetype", "easeOutBack", "onupdate", "SetAlertEffectInfoPos", "oncomplete", "EnableAlertEffectClick"));
    }

    public void HideAlertEffect()
    {
        HideAlertEffectInfo();
    }

    private void HideAlertEffectInfo()
    {
        AlertEffectInfoObject.transform.localPosition = new Vector3(0, AlertEffectInfoObject.transform.localPosition.y, 0);
        AlertEffectInfoObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", m_TotalWidthInCoordinate, "time", LabelMoveDuration, "easetype", "easeInBack", "onupdate", "SetAlertEffectInfoPos", "oncomplete", "HideAlertEffectBG"));
    }

    private void HideAlertEffectBG()
    {
        AlertEffectInfoObject.SetActive(false);

        AlertEffectBGObject.transform.localScale = new Vector3(1, 1, 1);
        AlertEffectBGObject.SetActive(true);
        iTween.ValueTo(gameObject, iTween.Hash("from", 1, "to", 0, "time", BGTurnDuration, "easetype", "easeInBack", "onupdate", "SetAlertEffectBGScale", "oncomplete", "EndAlertEffect"));
    }

    public void EndAlertEffect()
    {
        AlertEffectBGObject.SetActive(false);
        AlertEffectMainObject.SetActive(false);

        isAlertInfoEffectShowing = false;

        if (m_ExecuteAfterEnd != null)
        {
            m_ExecuteAfterEnd();
        }
    }

    public void SetAlertEffectBGScale(float value)
    {
        AlertEffectBGObject.transform.localScale = new Vector3(1, value, 1);
    }

    public void SetAlertEffectInfoPos(float value)
    {
        AlertEffectInfoObject.transform.localPosition = new Vector3(value, AlertEffectInfoObject.transform.localPosition.y, 0);
    }

    public void EnableAlertEffectClick()
    {
        m_isCanClickEffect = true;

        CycleTween.StartCycleTween(AlertEffectButtomInfoLabel.gameObject, 1, 0, 1.0f, OnUpdateAlertInfoLabelA);
    }

    public void DenableAlertEffectClick()
    {
        m_isCanClickEffect = false;
    }

    private void OnUpdateAlertInfoLabelA(float value)
    {
        AlertEffectButtomInfoLabel.color = new Color(AlertEffectButtomInfoLabel.color.r, AlertEffectButtomInfoLabel.color.g, AlertEffectButtomInfoLabel.color.b, value);
    }

    void Awake()
    {
        AlertEffectBGObject.GetComponent<UISprite>().width = (int)(m_TotalWidthInCoordinate);
    }
}