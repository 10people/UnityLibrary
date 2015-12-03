using System;
using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// Controller of personal player, this is not a abstract class cause some feature must be putted in, but your should always implement this to attach it on your prefab.
/// </summary>
public class SinglePlayerController : MonoBehaviour
{
    #region Configs

    /// <summary>
    /// Is fixed or rotate main camera.
    /// </summary>
    [HideInInspector]
    public bool IsRotateCamera;

    /// <summary>
    /// Is upload player position or not.
    /// </summary>
    [HideInInspector]
    public bool IsUploadPlayerPosition;

    [HideInInspector]
    public float BaseGroundPosY;

    /// <summary>
    /// Sync character if has duration has passed.
    /// </summary>
    [HideInInspector]
    public float m_CharacterSyncDuration = 0;

    #endregion

    #region Move Controller

    private bool is_CanMove = true;

    public void DeactiveMove()
    {
        StopPlayerNavigation();
        is_CanMove = false;
    }

    public void ActiveMove()
    {
        is_CanMove = true;
    }

    #endregion

    public virtual void OnPlayerRun()
    {
        Debug.LogError("OnPlayerRun not implemented in derived class");
    }

    public virtual void OnPlayerStop()
    {
        Debug.LogError("OnPlayerStop not implemented in derived class");
    }

    /// <summary>
    /// Character animations controller.
    /// </summary>
    public Animator m_Animator;

    #region Move and navigation

    /// <summary>
    /// quit navigation when use character control
    /// </summary>
    [HideInInspector]
    public bool IsInNavigate;

    //these delegates execute on specific timing in navigation.
    [HideInInspector]
    public delegate void VoidDelegate();
    [HideInInspector]
    public VoidDelegate m_CompleteNavDelegate;
    public VoidDelegate m_StartNavDelegate;
    public VoidDelegate m_StopNavDelegate;

    [HideInInspector]
    public Vector3 NavigationEndPosition;

    public CharacterController m_CharacterController;
    public NavMeshAgent m_NavMeshAgent;

    [HideInInspector]
    public Joystick m_Joystick;

    public Vector3 m_RealJoystickOffset
    {
        get
        {
            return
#if UNITY_EDITOR || UNITY_STANDALONE
                new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
#else
            m_Joystick.m_Offset;
#endif
        }
    }

    public float m_CharacterSpeed = 6;
    public float m_CharacterSpeedY = 0.6f;
    public float m_NavigateSpeed = 6;

    /// <summary>
    /// how much degree does angle change per second.
    /// </summary>
    [HideInInspector]
    public const float angleSpeed = 120;

    public Transform m_Transform;

    /// <summary>
    /// is moving means in navigation or character move.
    /// </summary>
    public bool m_IsMoving;

    /// <summary>
    /// Cannot start navi or use character control when turning.
    /// </summary>
    public bool m_IsTurning;

    public float m_LastNavigateTime;
    public Transform m_NavigationTransform;

    public void StartNavigation(Vector3 tempPosition)
    {
        if (!is_CanMove)
        {
            Debug.LogWarning("Cancel navigate cause controller set.");
            return;
        }

        if (m_RealJoystickOffset != Vector3.zero)
        {
            Debug.LogWarning("Cancel navigation cause in character control");
            return;
        }

        if (!m_IsTurning)
        {
            m_LastNavigateTime = Time.realtimeSinceStartup;

            m_IsTurning = true;
            StartCoroutine(DoStartNavigation(tempPosition));

            if (m_StartNavDelegate != null)
            {
                m_StartNavDelegate();
            }
        }
    }

    public IEnumerator DoStartNavigation(Vector3 tempPosition)
    {
        while (true)
        {
            Vector3 oldAngle = transform.eulerAngles;
            transform.forward = tempPosition - transform.position;

            float targetAngleY = transform.eulerAngles.y;
            float maxDelta = 1080 * Time.deltaTime;
            float angle = Mathf.MoveTowardsAngle(oldAngle.y, targetAngleY, maxDelta);

            transform.eulerAngles = new Vector3(0, angle, 0);

            if (Mathf.Abs(targetAngleY - oldAngle.y) < 20)
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        NavigationEndPosition = tempPosition;

        m_CharacterController.enabled = false;
        m_NavMeshAgent.enabled = true;

        OnPlayerRun();
        if (!m_IsMoving)
        {
            m_IsMoving = true;
        }

        Debug.Log("Start navigate to position:" + tempPosition);

        m_NavMeshAgent.Resume();

        m_NavMeshAgent.SetDestination(tempPosition);

        m_IsTurning = false;
        IsInNavigate = true;
    }

    public void StopPlayerNavigation()
    {
        if (!IsInNavigate)
        {
            return;
        }

        OnPlayerStop();
        if (m_IsMoving)
        {
            m_IsMoving = false;
        }

        m_NavMeshAgent.Stop();
        IsInNavigate = false;

        m_CharacterController.enabled = true;
        m_NavMeshAgent.enabled = false;

        if (m_StopNavDelegate != null)
        {
            m_StopNavDelegate();
        }
    }

    #endregion

    #region Character track camera

    [HideInInspector]
    public Camera TrackCamera;

    /// <summary>
    /// offset of positive axis y, you must set this before using this script.
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetPosUp;

    /// <summary>
    /// offset of negative axis z, you must set this before using this script.
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetPosBack;

    /// <summary>
    /// offset of up down rotation, you must set this before using this script.
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetUpDownRotation;

    public void LateUpdate()
    {
        if (TrackCamera == null)
        {
            return;
        }

        //Use camera back, up and updownrotate value.
        if (IsRotateCamera)
        {
            TrackCamera.transform.localPosition = transform.localPosition;
            TrackCamera.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
            TrackCamera.transform.Translate(Vector3.up * TrackCameraOffsetPosUp);
            TrackCamera.transform.Translate(Vector3.back * TrackCameraOffsetPosBack);
            TrackCamera.transform.localEulerAngles = new Vector3(TrackCameraOffsetUpDownRotation, transform.localEulerAngles.y, 0);
        }
        //Use camera offset position and rotation.
        else
        {
            TrackCamera.transform.localPosition = transform.localPosition + new Vector3(0, TrackCameraOffsetPosUp, -TrackCameraOffsetPosBack);
        }
    }

    #endregion

    #region Character sync

    public float m_lastUploadCheckTime;

    public Vector3 m_lastPosition;
    public Vector3 m_nowPosition;

    /// <summary>
    /// Commit this character's position to server.
    /// </summary>
    public void UploadPlayerPosition()
    {
        if (Vector3.Distance(m_lastPosition, m_nowPosition) > 0.1) //玩家有位移 发送数据
        {
            //TODO: Complete sending sync message to server.
        }
    }

    #endregion

    public void Update()
    {
        #region Character Sync

        if (IsUploadPlayerPosition && Time.realtimeSinceStartup - m_lastUploadCheckTime >= m_CharacterSyncDuration)
        {
            m_lastUploadCheckTime = Time.realtimeSinceStartup;
            m_nowPosition = transform.position;

            UploadPlayerPosition();
        }

        #endregion

        //TODO: Sync audio listener position with player, used in RPG game to make player hear the real sound.

        if (m_RealJoystickOffset != Vector3.zero && IsInNavigate)
        {
            StopPlayerNavigation();
        }

        #region Character Controller

        if (!is_CanMove)
        {
            Debug.LogWarning("Cancel move cause controller set.");
        }
        else
        {
            if (!m_IsTurning && !IsInNavigate)
            {
                Vector3 offset = m_RealJoystickOffset;

                if (offset != Vector3.zero)
                {
                    if (IsRotateCamera)
                    {
                        Vector3 normalizedOffset = offset.normalized;
                        bool isMoveForward = false;

                        double angleTemp = Math.Atan2(normalizedOffset.x, normalizedOffset.z) / Math.PI * 180;
                        double distance = Vector2.Distance(Vector2.zero, new Vector2(offset.x, offset.z));
                        if (60 < angleTemp && 180 > angleTemp)
                        {
                            m_Transform.localEulerAngles = new Vector3(0, (float)(m_Transform.localEulerAngles.y + angleSpeed * Time.deltaTime), 0);
                        }
                        else if (angleTemp > -180 && angleTemp < -60)
                        {
                            m_Transform.localEulerAngles = new Vector3(0, (float)(m_Transform.localEulerAngles.y - angleSpeed * Time.deltaTime), 0);
                        }
                        else if (angleTemp > -60 && angleTemp < 60)
                        {
                            isMoveForward = true;
                        }

                        OnPlayerRun();

                        if (!m_IsMoving)
                        {
                            m_IsMoving = true;
                        }

                        Vector3 moveDirection = transform.forward;

                        if (!m_CharacterController.isGrounded)
                        {
                            moveDirection.y -= m_CharacterSpeedY;
                        }

                        //move if offset above half distance
                        if (distance > Joystick.MaxReinDistance / 2.0f || isMoveForward)
                        {
                            m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        Vector3 moveDirection = offset.normalized;

                        OnPlayerRun();

                        if (!m_IsMoving)
                        {
                            m_IsMoving = true;
                        }

                        if (!m_CharacterController.isGrounded)
                        {
                            moveDirection.y -= m_CharacterSpeedY;
                        }

                        //rotate and move.
                        transform.forward = offset.normalized;
                        m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    OnPlayerStop();

                    if (m_IsMoving)
                    {
                        m_IsMoving = false;
                    }

                    Vector3 moveDirection = Vector3.zero;

                    if (!m_CharacterController.isGrounded)
                    {
                        moveDirection.y -= m_CharacterSpeedY;
                    }

                    m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                }
            }
        }

        #endregion

        #region Navigation Controller

        if (IsInNavigate)
        {
            //Check navigation remaining destination, end navigation if close enough.
            if (Vector3.Distance(m_Transform.position, NavigationEndPosition) <= 3)
            {
                StopPlayerNavigation();
                if (m_CompleteNavDelegate != null)
                {
                    m_CompleteNavDelegate();
                    m_CompleteNavDelegate = null;
                }
            }
        }

        #endregion
    }

    public void Start()
    {
        m_Transform = transform;
        m_CharacterController.enabled = true;
        m_NavMeshAgent.enabled = false;

        m_NavMeshAgent.speed = m_NavigateSpeed;
        m_NavMeshAgent.acceleration = 10000.0f;
    }

    public void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }
}
