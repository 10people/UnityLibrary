using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    public static float m_LatestServerSyncTime;

    public List<GameObject> PlayerPrefabList = new List<GameObject>();
    public GameObject PlayerParentObject;

    /// <summary>
    /// Key for id, value for OtherPlayerController
    /// </summary>
    public Dictionary<int, OtherPlayerController> m_PlayerDic = new Dictionary<int, OtherPlayerController>();

    public virtual void AddTrackCamera(OtherPlayerController temp)
    {
        Debug.LogError("Call AddTrackCamera in base class.");
    }

    public bool CreatePlayer(int l_roleID, int l_uID, Vector3 l_position,Transform p_parentTransform)
    {
        if (m_PlayerDic.ContainsKey(l_uID))
        {
            return false;
        }

        if (!m_PlayerDic.ContainsKey(l_uID))
        {
            var temp = Instantiate(PlayerPrefabList[l_roleID - 1]) as GameObject;

            temp.GetComponent<CharacterController>().enabled = false;
            temp.GetComponent<NavMeshAgent>().enabled = false;

            Utils.ActiveWithStandardize(p_parentTransform, temp.transform);

            temp.transform.localPosition = l_position;
            temp.transform.name = "CreatedOtherPlayer_" + l_uID;

            OtherPlayerController tempItem = temp.AddComponent<OtherPlayerController>();
            tempItem.m_CharacterLerpDuration = l_roleID >= 50000 ? 1.0f : 0.4f;
            AddTrackCamera(tempItem);

            tempItem.m_UID = l_uID;

            m_PlayerDic.Add(l_uID, tempItem);
        }

        return true;
    }

    public void UpdatePlayerTransform(int l_uID, Vector3 l_position, float l_y_Rotation)
    {
        if (!m_PlayerDic.ContainsKey(l_uID))
        {
            return;
        }

        OtherPlayerController tempPlayer = m_PlayerDic[l_uID];

        tempPlayer.StartPlayerTransformTurn(l_position, new Vector3(0, l_y_Rotation, 0), true);
    }

    public void DestroyPlayer(int l_uID)
    {
        if (m_PlayerDic.ContainsKey(l_uID))
        {
            Destroy(m_PlayerDic[l_uID].gameObject);
            m_PlayerDic.Remove(l_uID);
        }
    }

    public void OnDestroy()
    {
        m_PlayerDic.Clear();
    }
}
