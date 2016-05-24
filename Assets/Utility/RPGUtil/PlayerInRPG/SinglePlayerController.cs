//#define DEBUG_MODE

using System;
using UnityEngine;
using System.Collections;
using System.IO;

public class SinglePlayerController : MonoBehaviour
{
    //TODO: add ur own audio listenre.
    private AudioListener m_audioListener;

    #region Configs

    /// <summary>
    /// Is fixed or rotate main camera.
    /// </summary>
    public bool IsRotateCamera = false;

    /// <summary>
    /// Is upload player position or not.
    /// </summary>
    public bool IsUploadPlayerPosition = true;

    public float BaseGroundPosY;

    public static float m_CharacterSyncDuration = 0.2f;
    public static float m_CharacterMoveDistance = 0.1f;

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
    /// player uid used for server sync.
    /// </summary>
    public static int s_uid;

    /// <summary>
    /// Character stood and run animations.
    /// </summary>
    public Animator m_Animator;

    #region Move and navigation

    /// <summary>
    /// quit navigation when use character control
    /// </summary>
    [HideInInspector]
    public bool IsInNavigate;

    [HideInInspector]
    public delegate void VoidDelegate();
    [HideInInspector]
    public VoidDelegate m_CompleteNavDelegate;
    public VoidDelegate m_StartNavDelegate;
    public VoidDelegate m_EndNavDelegate;

    [HideInInspector]
    public Vector3 NavigationEndPosition;
    private float navigateDistance = 1f;

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
            m_Joystick.m_uiOffset;
#endif
        }
    }

    //TODO set ur own config
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tempPosition"></param>
    /// <param name="tempDistance">navigate distance, 1f for default.</param>
    public void StartNavigation(Vector3 tempPosition, float tempDistance = 1f)
    {
        if (!is_CanMove)
        {
#if DEBUG_MODE
            Debug.LogWarning("Cancel navigate cause controller set.");
#endif
            return;
        }

        if (m_RealJoystickOffset != Vector3.zero)
        {
#if DEBUG_MODE
            Debug.LogWarning("Cancel navigation cause in character control");
#endif
            return;
        }

        if (!m_IsTurning)
        {
            m_LastNavigateTime = Time.realtimeSinceStartup;

            m_IsTurning = true;

            navigateDistance = tempDistance;

            StartCoroutine(DoStartNavigation(tempPosition));

            if (m_StartNavDelegate != null)
            {
                m_StartNavDelegate();
            }
        }
    }

    private float navigationRotateSpeed = 180f;

    public IEnumerator DoStartNavigation(Vector3 tempPosition)
    {
        while (true)
        {
            Vector3 oldAngle = transform.eulerAngles;

            //Get target angle.
            transform.forward = tempPosition - transform.position;
            float targetAngleY = transform.eulerAngles.y;

            //Get angle per frame.
            float maxDelta = navigationRotateSpeed * Time.deltaTime;
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

        if (m_EndNavDelegate != null)
        {
            m_EndNavDelegate();
        }
    }

    #endregion

    #region Character track camera

    [HideInInspector]
    public Camera TrackCamera;

    /// <summary>
    /// offset of positive axis y, Only effective in camera rotate mode
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetPosUp;

    /// <summary>
    /// offset of negative axis z, Only effective in camera rotate mode
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetPosBack;

    /// <summary>
    /// offset of up down rotation, Only effective in camera rotate mode
    /// </summary>
    [HideInInspector]
    public float TrackCameraOffsetUpDownRotation;

    [HideInInspector]
    public Vector3 TrackCameraPosition;
    [HideInInspector]
    public Vector3 TrackCameraRotation;

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
            if (TrackCameraPosition == Vector3.zero || TrackCameraRotation == Vector3.zero)
            {
                TrackCameraPosition = TrackCamera.transform.localPosition;
                TrackCameraRotation = TrackCamera.transform.localEulerAngles;

                return;
            }

            TrackCamera.transform.localPosition = TrackCameraPosition + new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
            TrackCamera.transform.localEulerAngles = TrackCameraRotation;
        }
    }

    #endregion

    #region Character sync

    public float m_lastUploadCheckTime;
    private int m_sameTransformTimes;

    public Vector3 m_lastPosition;
    public Vector3 m_lastRotation;

    /// <summary>
    /// Commit this character's position to server.
    /// </summary>
    public void TryUploadPlayerPosition()
    {
        if (Vector3.Distance(m_lastPosition, transform.localPosition) < 0.1f && Vector3.Distance(m_lastRotation, transform.localEulerAngles) < 0.1f)
        {
            m_sameTransformTimes++;
        }
        else
        {
            m_sameTransformTimes = 0;
        }

        if (m_sameTransformTimes >= 2)
        {
            return;
        }


        //TODO: send position to server.

        m_lastPosition = transform.localPosition;
        m_lastRotation = transform.localEulerAngles;
    }

    #endregion

    public void OnDestroy()
    {
        StopPlayerNavigation();

        m_CharacterController = null;

        m_NavMeshAgent = null;

        m_Animator = null;
    }

    public void Update()
    {
        #region Character Sync

        if (IsUploadPlayerPosition && Time.realtimeSinceStartup - m_lastUploadCheckTime >= m_CharacterSyncDuration)
        {
            m_lastUploadCheckTime = Time.realtimeSinceStartup;

            TryUploadPlayerPosition();
        }

        #endregion

        m_audioListener.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);

        if (m_RealJoystickOffset != Vector3.zero && IsInNavigate)
        {
            StopPlayerNavigation();
        }

        #region Character Controller

        if (!is_CanMove)
        {
#if DEBUG_MODE
            Debug.Log("Cancel move cause controller set.");
#endif
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
                        if (distance > Joystick.MaxRadiusDistance / 2.0f || isMoveForward)
                        {
                            m_CharacterController.Move(moveDirection.normalized * m_CharacterSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        Vector3 moveDirection = offset.normalized;
                        double degree = -TrackCamera.transform.localEulerAngles.y * Math.PI / 180;
                        moveDirection = new Vector3((float)(Math.Cos(degree) * moveDirection.x - Math.Sin(degree) * moveDirection.z), 0, (float)(Math.Cos(degree) * moveDirection.z + Math.Sin(degree) * moveDirection.x));

                        OnPlayerRun();

                        if (!m_IsMoving)
                        {
                            m_IsMoving = true;
                        }

                        //rotate and move.
                        transform.forward = moveDirection.normalized;

                        if (!m_CharacterController.isGrounded)
                        {
                            moveDirection.y -= m_CharacterSpeedY;
                        }

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
            //Check navigation remaining destination
            if (Vector3.Distance(m_Transform.position, NavigationEndPosition) <= navigateDistance)
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
