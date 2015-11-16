using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Toggles controller, call OnToggleClick manully.
/// </summary>
public class TogglesControl : MonoBehaviour
{
    /// <summary>
    /// Listeners binded to toggle objects, you can only PLUS new delegate here!
    /// </summary>
    public List<EventIndexHandle> TogglesEvents;

    /// <summary>
    /// Widget depth move value.
    /// </summary>
    public int MoveDepth;

    /// <summary>
    /// Widget a change value.
    /// </summary>
    public float ChangeA;

    private GameObject TriggeredObject;

    /// <summary>
    /// Call this method manully.
    /// </summary>
    /// <param name="index"></param>
    public void OnToggleClick(int index)
    {
        TogglesEvents.ForEach(item => SetColorA(item.gameObject, ChangeA));

        if (MoveDepth != 0)
        {
            SetObjectsDepth(TogglesEvents[index].gameObject, MoveDepth, true);
        }
        SetColorA(TogglesEvents[index].gameObject, 1);

        TriggeredObject = TogglesEvents[index].gameObject;
    }


    private EventIndexHandle GetToggleEventByIndex(int index)
    {
        return TogglesEvents.Where(item => item.m_SendIndex == index).FirstOrDefault();
    }

    /// <summary>
    /// Set object and object's children widget depth forward or backward.
    /// </summary>
    /// <param name="go">the object</param>
    /// <param name="translateValue">changed depth</param>
    /// <param name="isForward">is depth forward</param>
    private void SetObjectsDepth(GameObject go, int translateValue, bool isForward)
    {
        var widgets = go.GetComponentsInChildren<UIWidget>(true);
        foreach (var widget in widgets)
        {
            if (isForward)
            {
                widget.depth += translateValue;
            }
            else
            {
                widget.depth -= translateValue;
            }
        }
    }

    /// <summary>
    /// Set object widget color a.
    /// </summary>
    /// <param name="go">the object</param>
    /// <param name="a">the value</param>
    private void SetColorA(GameObject go, float a)
    {
        var widget = go.GetComponent<UIWidget>();
        widget.color = new Color(widget.color.r, widget.color.g, widget.color.b, a);
    }
}
