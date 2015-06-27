using UnityEngine;

/// <summary>
/// Long press control for NGUI, OnLongPressFinish only for press type.
/// </summary>
public class NGUILongPress : MonoBehaviour
{
    #region Public Fields

    /// <summary>
    /// Only used for press type.
    /// </summary>
    public UIEventListener.VoidDelegate OnLongPressFinish;

    /// <summary>
    /// Long press trigger.
    /// </summary>
    public UIEventListener.VoidDelegate OnLongPress;

    /// <summary>
    /// Normal press trigger.
    /// </summary>
    public UIEventListener.VoidDelegate OnNormalPress;

    public enum TriggerType
    {
        Release,
        Press
    }
    [HideInInspector]
    public TriggerType LongTriggerType = TriggerType.Press;
    [HideInInspector]
    public bool NormalPressTriggerWhenLongPress = false;

    #endregion

    #region Private Fields

    private bool dragged;
    private float lastPress = -1f;
    private bool isInPress;
    private Vector3 cachedClickPos;
    private const float MinDeviation = 0.01f;
    private const float LongClickDuration = 0.2f;

    #endregion

    #region Private Methods

    private void OnPress(bool pressed)
    {
        if (pressed)
        {
            dragged = false;
            lastPress = Time.realtimeSinceStartup;
            isInPress = true;
            cachedClickPos = Input.mousePosition;
            if (LongTriggerType == TriggerType.Press)
            {
                Invoke("CheckPressTypeLongPress", LongClickDuration);
            }
        }
        else
        {
            isInPress = false;
            //If the press time is over long click duration and the object is not be dragged, trigger long press.
            if (Time.realtimeSinceStartup - lastPress > LongClickDuration)
            {
                CheckReleaseTypeLongPress();
                CheckPressTypeLongPressFinish();
            }
        }
    }

    private void OnClick()
    {
        isInPress = false;
        if (!NormalPressTriggerWhenLongPress)
        {
            if (Time.realtimeSinceStartup - lastPress < LongClickDuration)
            {
                CancelInvoke("CheckPressTypeLongPress");
                if (OnNormalPress != null)
                {
                    OnNormalPress(gameObject);
                }
            }
        }
        else
        {
            CancelInvoke("CheckPressTypeLongPress");
            if (OnNormalPress != null)
            {
                OnNormalPress(gameObject);
            }
        }
    }

    private void OnDragStart()
    {
        dragged = true;
        if (Vector3.Distance(cachedClickPos, Input.mousePosition) > MinDeviation
            && Time.realtimeSinceStartup - lastPress > LongClickDuration)
        {
            CheckPressTypeLongPressFinish();
        }
    }

    private void CheckReleaseTypeLongPress()
    {
        if (LongTriggerType == TriggerType.Release && !dragged && OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    private void CheckPressTypeLongPressFinish()
    {
        if (LongTriggerType == TriggerType.Press && OnLongPressFinish != null)
        {
            OnLongPressFinish(gameObject);
        }
    }

    private void CheckPressTypeLongPress()
    {
        if (!dragged && isInPress && OnLongPress != null)
        {
            OnLongPress(gameObject);
        }
    }

    #endregion
}
