using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FloatButtonsController : MonoBehaviour
{
    public GameObject m_ButtonPrefab;
    public UIGrid m_Grid;
    public GameObject m_BGLeft;
    public GameObject m_BGRight;

    public delegate void VoidDelegate();

    /// <summary>
    /// Float buttons config struct
    /// </summary>
    public struct ButtonInfo
    {
        /// <summary>
        /// Button label
        /// </summary>
        public string m_LabelStr;

        /// <summary>
        /// Button click delegate
        /// </summary>
        public VoidDelegate m_VoidDelegate;
    }
    public Dictionary<int, ButtonInfo> m_ButtonInfoDic = new Dictionary<int, ButtonInfo>();
    [HideInInspector]
    public Dictionary<int, GameObject> m_ButtonObjectDic = new Dictionary<int, GameObject>();

    /// <summary>
    /// Initialize float buttons.
    /// </summary>
    /// <param name="l_buttonInfos">button label and delegate setting</param>
    /// <param name="isLeft">left mode or right mode</param>
    /// <param name="addDepth">add depth to whole buttons to adjust ui</param>
    public void Initialize(List<ButtonInfo> l_buttonInfos, bool isLeft, int addDepth = 0)
    {
        while (m_Grid.transform.childCount != 0)
        {
            var child = m_Grid.transform.GetChild(0);
            child.parent = null;
            Destroy(child);
        }
        foreach (var item in m_ButtonObjectDic)
        {
            item.Value.transform.parent = null;
            Destroy(item.Value.gameObject);
        }
        m_ButtonObjectDic.Clear();
        m_ButtonInfoDic.Clear();

        for (int i = 0; i < l_buttonInfos.Count; i++)
        {
            var temp = Instantiate(m_ButtonPrefab) as GameObject;
            Utils.ActiveWithStandardize(m_Grid.transform, temp.transform);

            temp.name += "_" + Utils.FullNumWithZeroDigit(i, l_buttonInfos.Count.ToString().Length);

            var label = temp.GetComponentInChildren<UILabel>();
            label.text = l_buttonInfos[i].m_LabelStr;

            var eventIndex = temp.GetComponent<EventIndexHandle>();
            eventIndex.m_SendIndex = i;
            eventIndex.m_Handle += OnButtonsClick;

            m_ButtonObjectDic.Add(i, temp);
            m_ButtonInfoDic.Add(i, l_buttonInfos[i]);
        }

        m_Grid.Reposition();

        if (isLeft)
        {
            m_BGLeft.SetActive(true);
            m_BGRight.SetActive(false);

            var sprite = m_BGLeft.GetComponent<UISprite>();
            sprite.topAnchor.target = m_ButtonObjectDic[0].transform;
            sprite.leftAnchor.target = sprite.transform;
            sprite.rightAnchor.target = sprite.transform;
            sprite.bottomAnchor.target = m_ButtonObjectDic[l_buttonInfos.Count - 1].transform;
        }
        else
        {
            m_BGLeft.SetActive(false);
            m_BGRight.SetActive(true);

            var sprite = m_BGRight.GetComponent<UISprite>();
            sprite.topAnchor.target = m_ButtonObjectDic[0].transform;
            sprite.leftAnchor.target = sprite.transform;
            sprite.rightAnchor.target = sprite.transform;
            sprite.bottomAnchor.target = m_ButtonObjectDic[l_buttonInfos.Count - 1].transform;
        }

        if (addDepth != 0)
        {
            gameObject.GetComponentsInChildren<UIWidget>().ToList().ForEach(item => item.depth += addDepth);
        }
    }

    public void OnButtonsClick(int index)
    {
        if (!m_ButtonInfoDic.ContainsKey(index))
        {
            Debug.LogError("Error index:" + index + " in float button click.");
            return;
        }

        m_ButtonInfoDic[index].m_VoidDelegate();

        Destroy(gameObject);
    }
}
