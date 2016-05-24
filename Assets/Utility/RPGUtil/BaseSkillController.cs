using System;
using UnityEngine;
using System.Collections;

public class BaseSkillController : MonoBehaviour
{
    public bool IsShowCdEndEffect = false;

    public bool IsInCD = false;

    //TODO: set ur own config.
    public static float SharedCD = 0.5f;
    public float SelfCD = -1;
    public float StartCDTime = -1;
    public float ActivedCD = -1;

    //TODO: set ur own ui.
    public UIButton m_SkillButton;
    public UISprite m_SkillSprite;
    public UISprite m_ShieldSprite;
    public UILabel m_CDLabel;

    public DelegateUtil.VoidDelegate BaseSkillClickDelegate;

    public void OnClick()
    {
        if (BaseSkillClickDelegate != null)
        {
            BaseSkillClickDelegate();
        }
    }

    public void TryStartSharedCD()
    {
        if (SharedCD > GetRemainingCDTime)
        {
            ActivedCD = SharedCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            m_CDLabel.text = ActivedCD + "s";
            m_CDLabel.gameObject.SetActive(true);
        }
    }

    public void TryStartSelfCD()
    {
        if (SelfCD > GetRemainingCDTime)
        {
            ActivedCD = SelfCD;
            StartCDTime = Time.realtimeSinceStartup;

            IsInCD = true;
            m_ShieldSprite.gameObject.SetActive(true);

            m_CDLabel.text = ActivedCD + "s";
            m_CDLabel.gameObject.SetActive(true);
        }
    }

    public void StopCD()
    {
        //if cd ends.
        if (IsInCD && IsShowCdEndEffect)
        {
            //TODO: play cd ends effect.
        }

        IsInCD = false;
        m_ShieldSprite.gameObject.SetActive(false);
        m_CDLabel.gameObject.SetActive(false);
    }

    private float GetRemainingCDTime
    {
        get { return ActivedCD - (Time.realtimeSinceStartup - StartCDTime); }
    }

    public void Update()
    {
        var elapseTime = Time.realtimeSinceStartup - StartCDTime;
        if (elapseTime <= ActivedCD)
        {
            m_ShieldSprite.fillAmount = (1 - elapseTime / ActivedCD);
            m_CDLabel.text = (int)Math.Ceiling(ActivedCD - elapseTime) + "s";
        }
        else
        {
            StopCD();
        }
    }

    public void Awake()
    {
        m_SkillButton = (m_SkillButton ?? GetComponent<UIButton>()) ?? gameObject.AddComponent<UIButton>();
        m_SkillSprite = (m_SkillSprite ?? GetComponent<UISprite>()) ?? gameObject.AddComponent<UISprite>();
    }
}
