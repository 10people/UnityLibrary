//#define DEBUG_MOVE
//#define DEBUG_MODE

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OtherPlayerController : MonoBehaviour
{
    //TODO: set ur own
    private float limitMoveDuration;

    #region Move Controller

    private bool IsCanMove = true;
    private bool IsInMove = false;

    public void DeactiveMove()
    {
#if DEBUG_MODE
        Debug.LogWarning("--------------other deactive move.");
#endif

        PlayerStop();
        IsCanMove = false;
    }

    public void ActiveMove()
    {
#if DEBUG_MODE
        Debug.LogWarning("+++++++++++++++other active move.");
#endif

        IsCanMove = true;
    }

    #endregion

    public Camera TrackCamera;

    [HideInInspector]
    public int m_UID;
    [HideInInspector]
    public Transform m_transform;
    [HideInInspector]
    public NavMeshAgent m_Agent;
    [HideInInspector]
    public Animator m_animation;
    [HideInInspector]
    public CharacterController m_CharacterController;

    public float m_CharacterLerpDuration;

    void Awake()
    {
        m_animation = GetComponent<Animator>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        var precentFromMove = (Time.realtimeSinceStartup - m_startMoveTime) / m_CharacterLerpDuration;

        //Server sync check.
        if (Time.realtimeSinceStartup - PlayerManager.m_LatestServerSyncTime > limitMoveDuration)
        {
#if DEBUG_MOVE
            Debug.LogWarning("++++++++++++Limit other move" + Time.realtimeSinceStartup + "uid:" + m_UID);
#endif
            return;
        }

        //Optimize character sync in client.
        if (precentFromMove > 1 && IsInMove)
        {
            if (m_animation != null)
            {
                m_animation.SetBool("Move", true);
            }

            var tempPosition = m_targetPosition * 2 - (isRealServerData ? m_lastTargetServerPosition : m_lastTargetPosition);
            var tempRotation = m_targetRotation * 2 - (isRealServerData ? m_lastTargetServerRotation : m_lastTargetRotation);

#if DEBUG_MOVE
            Debug.LogWarning("==========Forcast other move, time:" + Time.realtimeSinceStartup + "uid:" + m_UID);
#endif

            StartPlayerTransformTurn(tempPosition, tempRotation, false);
        }

        if (precentFromMove >= 0 && precentFromMove <= 1 && IsCanMove)
        {
            IsInMove = true;

            if (m_animation != null)
            {
                m_animation.SetBool("Move", true);
            }

            {
                transform.localPosition = Vector3.Lerp(m_tryStartMovePosition, m_targetPosition, (float)precentFromMove);
            }

            if (Math.Abs(m_tryStartMoveRotation.y - m_targetRotation.y) > 180)
            {
                m_tryStartMoveRotation = new Vector3(m_tryStartMoveRotation.x, (m_tryStartMoveRotation.y < m_targetRotation.y ? (m_tryStartMoveRotation.y + 360) : (m_tryStartMoveRotation.y - 360)), m_tryStartMoveRotation.z);
            }

            transform.localEulerAngles = Vector3.Lerp(m_tryStartMoveRotation, m_targetRotation, (float)precentFromMove);
        }
    }

    private Vector3 m_lastTargetPosition;
    private Vector3 m_lastTargetRotation;
    private Vector3 m_lastTargetServerPosition;
    private Vector3 m_lastTargetServerRotation;

    private Vector3 m_targetPosition;
    private Vector3 m_targetRotation;
    private Vector3 m_targetServerPosition;
    private Vector3 m_targetServerRotation;
    private Vector3 m_tryStartMovePosition;
    private Vector3 m_tryStartMoveRotation;

    private float m_startMoveTime;
    private bool isRealServerData = false;

    public void StartPlayerTransformTurn(Vector3 p_targetPosition, Vector3 p_targetRotation, bool isRealServerData)
    {
        if (!IsCanMove)
        {
#if DEBUG_MODE
            Debug.Log("Cancel move cause controller set.");
#endif
            return;
        }

        this.isRealServerData = isRealServerData;

        //Record now transform
        m_tryStartMovePosition = transform.localPosition;
        m_tryStartMoveRotation = transform.localEulerAngles;

        //Set target transform and record last target transform.
        m_lastTargetPosition = m_targetPosition;
        m_targetPosition = new Vector3(p_targetPosition.x, m_tryStartMovePosition.y, p_targetPosition.z);
        if (isRealServerData)
        {
            m_lastTargetServerPosition = m_targetServerPosition;
            m_targetServerPosition = m_targetPosition;
        }

        m_lastTargetRotation = m_targetRotation;
        m_targetRotation = p_targetRotation;
        if (isRealServerData)
        {
            m_lastTargetServerRotation = m_targetServerRotation;
            m_targetServerRotation = p_targetRotation;
        }

        if ((Vector3.Distance(m_tryStartMovePosition, m_targetPosition) < SinglePlayerController.m_CharacterMoveDistance && Vector3.Distance(m_tryStartMoveRotation, m_targetRotation) < SinglePlayerController.m_CharacterMoveDistance))
        {
            //Stop player.
            PlayerStop();
        }
        else
        {
            //Set now time.
            m_startMoveTime = Time.realtimeSinceStartup - Time.deltaTime;
        }
    }

    public void PlayerStop()
    {
        IsInMove = false;

        m_targetPosition = m_tryStartMovePosition = transform.localPosition;
        m_targetRotation = m_tryStartMoveRotation = transform.localEulerAngles;

        if (m_animation != null)
        {
            m_animation.SetBool("Move", false);
        }
    }

    public void DestoryObject()
    {
        Destroy(gameObject);
    }
}
