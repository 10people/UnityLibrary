#define DEBUG_MODE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

public class AnimationHierarchyPlayer
{
    //TODO: set ur own.
    private int m_MyselfUid;

    public PlayerManager m_PlayerManager;
    public SinglePlayerController m_SinglePlayerController;
    public Dictionary<int, int> m_PiorityDic = new Dictionary<int, int>();

    public Animator TryGetAnimator(int p_uid)
    {
        if (p_uid == m_MyselfUid)
        {
            if (m_SinglePlayerController == null)
            {
#if DEBUG_MODE
                Debug.LogWarning("self player not exist");
#endif
                return null;
            }
            return m_SinglePlayerController.GetComponent<Animator>();
        }
        else
        {
            if (!m_PlayerManager.m_PlayerDic.ContainsKey(p_uid))
            {
#if DEBUG_MODE
                Debug.LogWarning("other player not exist");
#endif
                return null;
            }
            return m_PlayerManager.m_PlayerDic[p_uid].GetComponent<Animator>();
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="p_uid"></param>
    /// <param name="animationName">play animation using this name</param>
    /// <param name="isStrictlyPlay">donot consider hierarchy if true</param>
    /// <returns></returns>
    public void TryPlayAnimationInAnimator(int p_uid, string animationName, bool isStrictlyPlay = false)
    {
        Animator l_animator = TryGetAnimator(p_uid);

        if (!IsCanPlayAnimationInAnimator(p_uid, animationName, isStrictlyPlay))
        {
            return;
        }

        if (l_animator == null)
        {
            Debug.LogWarning("Cannot play animation cause cannot find player, " + p_uid);
            return;
        }

        l_animator.Play(animationName);
    }

    public bool IsCanPlayAnimationInAnimator(int p_uid, string animationName, bool isStrictlyPlay = false)
    {
        Animator l_animator = TryGetAnimator(p_uid);

        if (l_animator == null)
        {
            Debug.LogWarning("Cannot play animation cause cannot find player, " + p_uid);
            return false;
        }

        if (isStrictlyPlay)
        {
#if DEBUG_MODE
            Debug.LogWarning("Cancel check hierarchy in strictly playing mode.");
#endif
        }
        else
        {
            if (m_PiorityDic[Animator.StringToHash(animationName)] < 0 || m_PiorityDic[Utils.GetAnimatorPlayingHash(l_animator)] < 0)
            {
#if DEBUG_MODE
                Debug.LogWarning("Cannot play animation cause animation/ current playing animation not exist in hierarchy.");
                return false;
#endif
            }
            if (m_PiorityDic[Animator.StringToHash(animationName)] > m_PiorityDic[Utils.GetAnimatorPlayingHash(l_animator)])
            {
#if DEBUG_MODE
                Debug.LogWarning("Cannot play animation: " + animationName + " in: " + p_uid + " cause animation hierarchy block, " + m_PiorityDic[Animator.StringToHash(animationName)] + ">" + m_PiorityDic[Utils.GetAnimatorPlayingHash(l_animator)]);
#endif
                return false;
            }
        }

        return true;
    }
}
