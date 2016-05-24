using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    public UITexture MapBG;
    public Texture SmallMapTexture;
    public Texture BigMapTexture;

    public Vector4 MapBorderRange
    {
        get { return m_IsMapInSmallMode ? SmallMapBorderRange : BigMapBorderRange; }
    }

    //unity config.
    public Vector4 SmallMapBorderRange;
    public Vector4 BigMapBorderRange;

    public DelegateUtil.VoidDelegate m_ExecuteAfterOpenMap;
    public DelegateUtil.VoidDelegate m_ExecuteAfterCloseMap;

    #region Trans animation

    public bool m_IsMapInSmallMode = true;

    private const float m_MapTransDuration = 0.5f;
    public float m_MapTransTime;

    public Transform m_MapSmallModeTransform;
    private float m_MapSmallModeA = 1.0f;
    public Transform m_MapBigModeTransform;
    private float m_MapBigModeA = 1.0f;

    public GameObject m_BigMapDeco;

    public void OnOpenBigMap()
    {
        if (!m_IsMapInSmallMode) return;

        m_MapTransTime = Time.realtimeSinceStartup;
        m_IsMapInSmallMode = false;

        MapBG.mainTexture = BigMapTexture;
        //MapBG.width = MapBG.height = 200;

        var color = GetComponent<UITexture>().color;
        GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapBigModeA);
        m_BigMapDeco.SetActive(true);

        if (m_ExecuteAfterOpenMap != null)
        {
            m_ExecuteAfterOpenMap();
        }
    }

    public void OnCloseBigMap()
    {
        if (m_IsMapInSmallMode) return;

        m_MapTransTime = Time.realtimeSinceStartup;
        m_IsMapInSmallMode = true;

        MapBG.mainTexture = SmallMapTexture;
        //m_SmallMapController.MapBG.width = m_SmallMapController.MapBG.height = 200;

        var color = GetComponent<UITexture>().color;
        GetComponent<UITexture>().color = new Color(color.r, color.g, color.b, m_MapSmallModeA);
        m_BigMapDeco.SetActive(false);

        if (m_ExecuteAfterCloseMap != null)
        {
            m_ExecuteAfterCloseMap();
        }
    }

    #endregion

    #region Been Attack Effect

    public MapBeenAttackEffectController m_MapBeenAttackEffectController;

    public void ShowBeenAttackEffect(int p_uid)
    {
        if (m_ItemGizmosDic.ContainsKey(p_uid))
        {
            m_MapBeenAttackEffectController.BlinkEffect(p_uid, m_ItemGizmosDic[p_uid].localPosition);
        }
    }

    #endregion

    #region Map gizmos

    public List<GameObject> m_ItemGizmosPrefabList = new List<GameObject>();
    public Dictionary<int, Transform> m_ItemGizmosDic = new Dictionary<int, Transform>();

    /// <summary>
    /// </summary>
    /// <param name="p_uID"></param>
    /// <param name="p_type">from 0 - 5</param>
    /// <param name="p_position"></param>
    /// <param name="p_rotation"></param>
    /// <returns></returns>
    public bool AddGizmos(int p_uID, int p_type, Vector3 p_position, float p_rotation)
    {
        if (m_ItemGizmosDic.ContainsKey(p_uID))
        {
            Debug.LogWarning("Cannot add duplicated gizmos to small map, id:" + p_uID);
            return false;
        }

        var gizmos = Instantiate(m_ItemGizmosPrefabList[p_type]);
        gizmos.name += "_" + p_uID;
        Utils.ActiveWithStandardize(transform, gizmos.transform);
        m_ItemGizmosDic.Add(p_uID, gizmos.transform);

        UpdateGizmosPosition(p_uID, p_position, p_rotation);

        return true;
    }

    public bool RemoveGizmos(int p_uID)
    {
        if (!m_ItemGizmosDic.ContainsKey(p_uID))
        {
            Debug.LogWarning("Cannot remove non-existed gizmos from small map, id:" + p_uID);
            return false;
        }

        Destroy(m_ItemGizmosDic[p_uID].gameObject);
        m_ItemGizmosDic.Remove(p_uID);

        return true;
    }

    public void UpdateGizmosPosition(int p_uID, Vector3 p_position, float p_rotation)
    {
        if (!m_ItemGizmosDic.ContainsKey(p_uID))
        {
            //Debug.LogWarning("Cannot update non-existed gizmos in small map, id:" + p_uID);
            return;
        }

        SetGizmosPosition(m_ItemGizmosDic[p_uID], p_position, p_rotation);
    }

    public void SetGizmosPosition(Transform target, Vector3 position, float p_rotation)
    {
        target.localPosition = GizmosPositionTransfer(position);
        target.localEulerAngles = new Vector3(0, 0, -p_rotation);
    }

    /// <summary>
    /// Transfer position in world space to small map space.
    /// </summary>
    /// <param name="originalPosition">aixs y not considered</param>
    /// <returns>small map position</returns>
    public Vector3 GizmosPositionTransfer(Vector3 originalPosition)
    {
        var percentVector2 = new Vector2((originalPosition.x - MapBorderRange.x) / (MapBorderRange.y - MapBorderRange.x), (originalPosition.z - MapBorderRange.z) / (MapBorderRange.w - MapBorderRange.z));

        return new Vector3(-MapBG.width / 2.0f + MapBG.width * percentVector2.x, -MapBG.height / 2.0f + MapBG.height * percentVector2.y, 0);
    }

    #endregion

    void Update()
    {
        #region Trans animation

        if (Time.realtimeSinceStartup - m_MapTransTime <= m_MapTransDuration)
        {
            if (!m_IsMapInSmallMode)
            {
                transform.position = Vector3.Lerp(m_MapSmallModeTransform.position, m_MapBigModeTransform.position, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                transform.localScale = Vector3.Lerp(m_MapSmallModeTransform.localScale, m_MapBigModeTransform.localScale, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
            }
            else
            {
                transform.position = Vector3.Lerp(m_MapBigModeTransform.position, m_MapSmallModeTransform.position, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
                transform.localScale = Vector3.Lerp(m_MapBigModeTransform.localScale, m_MapSmallModeTransform.localScale, (Time.realtimeSinceStartup - m_MapTransTime) / m_MapTransDuration);
            }
        }

        #endregion
    }

    void OnDestroy()
    {
        SmallMapTexture = null;

        BigMapTexture = null;
    }
}
