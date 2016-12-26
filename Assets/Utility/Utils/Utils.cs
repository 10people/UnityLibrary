//----------------------------------------------
//	Time        : 
//	Author      : Taylor Liang
//	Site        : https://github.com/10people
//	Instruction : 
//	ChangeLog   :
//
//	Copyright © 2011-2016 Taylor Liang
//----------------------------------------------

using System;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Utils : Singleton<Utils>
{
    #region Transform

    /// <summary>
    /// Ergodic parent's all children
    /// </summary>
    /// <param name="parent">parent</param>
    /// <returns>all children</returns>
    public static List<Transform> ErgodicChilds(Transform parent)
    {
        List<Transform> returnTransforms = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            returnTransforms.Add(parent.GetChild(i));
        }

        foreach (var item in returnTransforms)
        {
            returnTransforms = returnTransforms.Concat(ErgodicChilds(item)).ToList();
        }

        return returnTransforms;
    }

    /// <summary>
    /// Ergodic child's all parents
    /// </summary>
    /// <param name="child">child</param>
    /// <returns>all parents</returns>
    public static List<Transform> ErgodicParents(Transform child)
    {
        if (child == null)
        {
            return null;
        }

        List<Transform> returnTransforms = new List<Transform>();
        Transform targetTransform = child.parent;
        while (targetTransform != null)
        {
            returnTransforms.Add(targetTransform);
            targetTransform = targetTransform.parent;
        }

        return returnTransforms;
    }

    /// <summary>
    /// Find the first child transform with special name. 
    /// </summary>
    /// <param name="parent">The parent tranfrom of the child which will be found.</param>
    /// <param name="objName">The name of the child transfrom.</param>
    /// <returns>The transfrom to be found, null if not found.</returns>
    public static Transform FindChild(Transform parent, string objName)
    {
        if (parent.name == objName)
        {
            return parent;
        }
        return (from Transform item in parent select FindChild(item, objName)).FirstOrDefault(child => child != null);
    }

    /// <summary>
    /// Find the first parent transform with special name. 
    /// </summary>
    /// <param name="child">The child tranfrom of the parent which will be found.</param>
    /// <param name="objName">The name of the child transfrom.</param>
    /// <returns>The transfrom to be found, null if not found.</returns>
    public static Transform FindParent(Transform child, string objName)
    {
        if (child == null)
        {
            return null;
        }
        return child.name == objName ? child : FindParent(child.parent.transform, objName);
    }

    /// <summary>
    /// Get the first parent specific component, for unity elder version in used, don't use GameObject.GetComponentInParent().
    /// </summary>
    /// <typeparam name="T">generic variable which inherited from monobehaviour</typeparam>
    /// <param name="child">The child tranfrom.</param>
    /// <returns>The component to be found, null if not found.</returns>
    public static T GetComponentInParent<T>(Transform child) where T : MonoBehaviour
    {
        if (child == null)
        {
            return null;
        }
        return child.GetComponent<T>() ?? GetComponentInParent<T>(child.parent.transform);
    }

    /// <summary>
    /// Set parent's child num to specific num, standardize automaticlly.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="prefabObject">child prefab</param>
    /// <param name="num">specific num</param>
    public static void AddOrDelItem(Transform parentTransform, GameObject prefabObject, int num)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:" + num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount != num)
            {
                var child = parentTransform.GetChild(0);
                child.parent = null;
                Destroy(child.gameObject);
            }
        }
        else if (parentTransform.childCount < num)
        {
            while (parentTransform.childCount != num)
            {
                var child = Instantiate(prefabObject) as GameObject;

                if (child == null)
                {
                    Debug.LogError("Fail to instantiate prefab, abort.");
                    return;
                }

                ActiveWithStandardize(parentTransform, child.transform);
            }
        }
    }

    /// <summary>
    /// Set parent's child num to specific num, using pool manager, standardize automaticlly.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="num">specific num</param>
    /// <param name="poolList">pool list</param>
    /// <param name="poolPrefabKey">which pool prefab to use</param>
    public static void AddOrDelItemUsingPool(Transform parentTransform, int num, PoolManagerListController poolList, string poolPrefabKey)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:" + num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount != num)
            {
                var child = parentTransform.GetChild(0);
                child.parent = null;
                poolList.ReturnItem(poolPrefabKey, child.gameObject);
            }
        }
        else if (parentTransform.childCount < num)
        {
            while (parentTransform.childCount != num)
            {
                var child = poolList.TakeItem(poolPrefabKey);

                if (child == null)
                {
                    Debug.LogError("Fail to instantiate prefab, abort.");
                    return;
                }

                ActiveWithStandardize(parentTransform, child.transform);
            }
        }
    }

    /// <summary>
    /// Set default transform and active.
    /// </summary>
    /// <param name="parent">parent transform</param>
    /// <param name="targetChild">transform standardized</param>
    public static void ActiveWithStandardize(Transform parent, Transform targetChild)
    {
        targetChild.transform.parent = parent;
        targetChild.transform.localPosition = Vector3.zero;
        targetChild.transform.localEulerAngles = Vector3.zero;
        targetChild.transform.localScale = Vector3.one;
        targetChild.gameObject.SetActive(true);
    }

    public static Vector3 GetTrackRotation(Vector3 sourcePos, Vector3 targetPos)
    {
        double angleTemp = Math.Atan2(targetPos.x - sourcePos.x, targetPos.z - sourcePos.z) / Math.PI * 180;

        return new Vector3(0, (float)angleTemp, 0);
    }

    #endregion

    #region Math

    public static string FullNumWithZeroDigit(int ori, int maxLength)
    {
        int length = ori.ToString().Length;
        int zeroToAdd = maxLength - length;

        if (zeroToAdd < 0)
        {
            Debug.LogError("Length error when full number.");
            return null;
        }

        string returnStr = "";

        while (zeroToAdd > 0)
        {
            returnStr += "0";
            zeroToAdd--;
        }
        returnStr += ori;

        return returnStr;
    }

    /// <summary>
    /// Get string bytes.
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GetBytesNumOfString(string str)
    {
        byte[] bytes = System.Text.Encoding.Unicode.GetBytes(str);
        int n = 0;
        for (int i = 0; i < bytes.GetLength(0); i++)
        {
            if (i % 2 == 0)
            {
                n++;
            }
            else
            {
                if (bytes[i] > 0)
                {
                    n++;
                }
            }
        }
        return n;
    }

    /// <summary>
    /// Get string before index by bytes.
    /// </summary>
    /// <param name="origStr">original string</param>
    /// <param name="Index">get bytes before index</param>
    /// <returns>cutted string</returns>
    public static string GetSubStringWithByteIndex(string origStr, int Index)
    {
        if (string.IsNullOrEmpty(origStr) || Index < 0)
        {
            return null;
        }
        int bytesCount = System.Text.Encoding.GetEncoding("utf-8").GetByteCount(origStr);
        if (bytesCount > Index)
        {
            int readyLength = 0;
            for (int i = 0; i < origStr.Length; i++)
            {
                var byteLength = System.Text.Encoding.GetEncoding("utf-8").GetByteCount(new char[] { origStr[i] });
                readyLength += byteLength;
                if (readyLength == Index)
                {
                    origStr = origStr.Substring(0, i + 1);
                    break;
                }
                else if (readyLength > Index)
                {
                    origStr = origStr.Substring(0, i);
                    break;
                }
            }
        }
        return origStr;
    }

    public static void SwapValue(ref float a, ref float b)
    {
        var temp = a;
        a = b;
        b = temp;
    }

    #endregion

    #region ScrollView Methods

    public static void SetScrollBarValue(UIScrollView view, UIScrollBar bar, float var)
    {
        if (var > 1 || var < 0)
        {
            Debug.LogError("Error setting in scroll bar.");
            return;
        }

        view.UpdateScrollbars(true);
        bar.value = var;
    }

    /// <summary>
    /// Adapt widget in verticle scroll view.
    /// </summary>
    /// <param name="scrollView">scroll view</param>
    /// <param name="scrollBar">scroll bar</param>
    /// <param name="widget">widget</param>
    public static void AdaptWidgetInScrollView(UIScrollView scrollView, UIScrollBar scrollBar, UIWidget widget)
    {
        //adapt pop up buttons to scroll view.
        float widgetValue = scrollView.GetWidgetValueRelativeToScrollView(widget).y;
        if (widgetValue < 0 || widgetValue > 1)
        {
            scrollView.SetWidgetValueRelativeToScrollView(widget, 0);

            //clamp scroll bar value.
            //donot update scroll bar cause SetWidgetValueRelativeToScrollView has updated.
            //set 0.99 and 0.01 cause same bar value not taken in execute.
            float scrollValue = scrollView.GetSingleScrollViewValue();
            if (scrollValue >= 1) scrollBar.value = scrollBar.value == 1.0f ? 0.99f : 1.0f;
            if (scrollValue <= 0) scrollBar.value = scrollBar.value == 0f ? 0.01f : 0f;
        }
    }

    #endregion

    #region Fold Line

    public class SegmentInFoldLine
    {
        public Vector2 StartPoint;
        public Vector2 EndPoint;
        public float Distance;
        public float PerviousPercentInTotal;
        public float Percent;
    }

    public static Vector2 GetPointFromFoldLine(float precent, List<Vector2> positionList)
    {
        List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
        if (temp == null)
        {
            return Vector2.zero;
        }

        return GetPointFromSegmentLine(precent, temp);
    }

    public static List<SegmentInFoldLine> GetSegmentListFromFoldLine(List<Vector2> positionList)
    {
        if (positionList == null || positionList.Count < 2)
        {
            Debug.LogError("Cannot get point cause position number less than 2.");
            return null;
        }

        List<SegmentInFoldLine> temp = new List<SegmentInFoldLine>();
        for (int i = 0; i < positionList.Count - 1; i++)
        {
            temp.Add(new SegmentInFoldLine()
            {
                StartPoint = positionList[i],
                EndPoint = positionList[i + 1],
            });
        }

        temp.ForEach(item => item.Distance = Vector2.Distance(item.StartPoint, item.EndPoint));
        float TotalDistance = temp.Select(item => item.Distance).Sum();
        temp.ForEach(item => item.Percent = item.Distance / TotalDistance);

        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].PerviousPercentInTotal = i > 0 ? (temp[i - 1].Percent + temp[i - 1].PerviousPercentInTotal) : 0;
        }

        return temp;
    }

    public static Vector2 GetPointFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
    {
        SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();

        //Debug.LogWarning(precent + "," + (precent - tempLine.PerviousPercentInTotal)/tempLine.Percent + "," + tempLine.StartPoint);
        return Vector2.Lerp(tempLine.StartPoint, tempLine.EndPoint, (precent - tempLine.PerviousPercentInTotal) / tempLine.Percent);
    }

    public static Vector2 GetDirectionFromFoldLine(float precent, List<Vector2> positionList)
    {
        List<SegmentInFoldLine> temp = GetSegmentListFromFoldLine(positionList);
        if (temp == null)
        {
            return Vector2.zero;
        }

        return GetDirectionFromSegmentLine(precent, temp);
    }

    public static Vector2 GetDirectionFromSegmentLine(float precent, List<SegmentInFoldLine> segmentList)
    {
        SegmentInFoldLine tempLine = segmentList.Where(item => item.PerviousPercentInTotal <= precent).OrderBy(item2 => item2.PerviousPercentInTotal).Last();
        return tempLine.EndPoint - tempLine.StartPoint;
    }

    #endregion

    #region ClockTime

    public struct ClockTime
    {
        public ClockTime(int l_hour, int l_minute, int l_second)
        {
            hour = l_hour;
            minute = l_minute;
            second = l_second;
        }

        public static ClockTime zero = new ClockTime(0, 0, 0);

        public int hour;
        public int minute;
        public int second;

        public static ClockTime Parse(string text)
        {
            try
            {
                var splited = text.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).Select(item => int.Parse(item)).ToList();
                if (splited.Count == 0) throw new Exception("ClockTime length is 0, transfer fail.");
                if (splited.Count == 1) return new ClockTime(0, 0, splited[0]);
                if (splited.Count == 2) return new ClockTime(0, splited[1], splited[0]);
                if (splited.Count == 3) return new ClockTime(splited[2], splited[1], splited[0]);
                throw new Exception("ClockTime length larger than 3, transfer fail.");
            }
            catch (Exception e)
            {
                Debug.LogError("Transfer ClockTime fail, contact LiangXiao if you donot know how to solve this, exception:" + e.Source + "\nstackTrace:" + e.StackTrace);
                return zero;
            }
        }

        public override string ToString()
        {
            string hourStr = hour.ToString();
            while (hourStr.Length < 2)
            {
                hourStr = "0" + hourStr;
            }

            string minuteStr = minute.ToString();
            while (minuteStr.Length < 2)
            {
                minuteStr = "0" + minuteStr;
            }

            string secondStr = second.ToString();
            while (secondStr.Length < 2)
            {
                secondStr = "0" + secondStr;
            }

            if (hour == 0)
            {
                return minuteStr + ":" + secondStr;
            }
            else
            {
                return hourStr + ":" + minuteStr + ":" + secondStr;
            }
        }
    }

    public static ClockTime SecondToClockTime(int second)
    {
        if (second < 0)
        {
            return ClockTime.zero;
        }

        return new ClockTime(second / 3600, (second % 3600) / 60, second % 60);
    }

    public static int ClockTimeToSecond(ClockTime clockTime)
    {
        return clockTime.hour * 3600 + clockTime.minute * 60 + clockTime.second;
    }

    #endregion

    #region Time Calculate Module

    private float TimeCalcLastTime;

    // Update is called once per frame
    void Update()
    {
        //One delegate.
        var tempList = TimeCalc.m_timeCalcList.Where(item => (item.DelegateMode == 1) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();
        for (int i = 0; i < tempList.Count; i++)
        {
            if (Time.realtimeSinceStartup - tempList[i].StartTime > tempList[i].OverTimeDuration)
            {
                tempList[i].IsOverTime = true;

                if (tempList[i].m_TimeCalcVoidDelegate != null)
                {
                    tempList[i].m_TimeCalcVoidDelegate();
                }
            }
        }

        //One Key string delegate.
        var tempList2 = TimeCalc.m_timeCalcList.Where(item => (item.DelegateMode == 2) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();
        for (int i = 0; i < tempList2.Count; i++)
        {
            if (Time.realtimeSinceStartup - tempList2[i].StartTime > tempList2[i].OverTimeDuration)
            {
                tempList2[i].IsOverTime = true;

                if (tempList2[i].m_TimeCalcStringDelegate != null)
                {
                    tempList2[i].m_TimeCalcStringDelegate(tempList2[i].key);
                }
            }
        }

        //Per/s delegate.
        if (Time.realtimeSinceStartup - TimeCalcLastTime > 1.0f)
        {
            var tempList3 = TimeCalc.m_timeCalcList.Where(item => (item.DelegateMode == 3) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();

            for (int i = 0; i < tempList3.Count; i++)
            {
                if (Time.realtimeSinceStartup - tempList3[i].StartTime > tempList3[i].OverTimeDuration)
                {
                    tempList3[i].IsOverTime = true;
                }

                if (tempList3[i].m_TimeCalcIntDelegate != null)
                {
                    tempList3[i].m_TimeCalcIntDelegate((int)GetCalcTime(tempList3[i].key));
                }
            }

            TimeCalcLastTime = Time.realtimeSinceStartup;
        }

        //Per/frame delegate.
        var tempList4 = TimeCalc.m_timeCalcList.Where(item => (item.DelegateMode == 4) && (item.StartTime >= 0) && (!item.IsOverTime)).ToList();

        for (int i = 0; i < tempList4.Count; i++)
        {
            if (Time.realtimeSinceStartup - tempList4[i].StartTime > tempList4[i].OverTimeDuration)
            {
                tempList4[i].IsOverTime = true;
            }

            if (tempList4[i].m_TimeCalcFloatDelegate != null)
            {
                tempList4[i].m_TimeCalcFloatDelegate(GetCalcTime(tempList4[i].key));
            }
        }
    }

    private class TimeCalc
    {
        public string key;

        public bool IsOverTime;
        public float StartTime;
        public float OverTimeDuration;
        /// <summary>
        /// 1: one deleagte, 2:per/s delegate, 3:per/frame delegate
        /// </summary>
        public int DelegateMode;
        public DelegateUtil.VoidDelegate m_TimeCalcVoidDelegate;
        public DelegateUtil.StringDelegate m_TimeCalcStringDelegate;
        public DelegateUtil.IntDelegate m_TimeCalcIntDelegate;
        public DelegateUtil.FloatDelegate m_TimeCalcFloatDelegate;

        public static bool ContainsKey(string key)
        {
            return m_timeCalcList.Any(item => item.key == key);
        }

        public static TimeCalc GetTimeCalc(string key)
        {
            if (!ContainsKey(key)) return null;

            return m_timeCalcList.Where(item => item.key == key).FirstOrDefault();
        }

        public static List<TimeCalc> m_timeCalcList = new List<TimeCalc>();
    }

    public delegate void TimeCalcVoidDelegate();
    public delegate void TimeCalcIntDelegate(int time);
    public delegate void TimeCalcFloatDelegate(float time);

    /// <summary>
    /// Add time calc dictionary item, delegate execute when time calc ends.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_voidDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddOneDelegateToTimeCalc(string key, float duration, DelegateUtil.VoidDelegate l_voidDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcVoidDelegate = l_voidDelegate, DelegateMode = 1 });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// Add time calc dictionary item, delegate execute when time calc ends.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_stringDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddOneDelegateToTimeCalc(string key, float duration, DelegateUtil.StringDelegate l_stringDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcStringDelegate = l_stringDelegate, DelegateMode = 2 });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// Add time calc dictionary item, delegate execute every seconds.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_intDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddEveryDelegateToTimeCalc(string key, float duration, DelegateUtil.IntDelegate l_intDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcIntDelegate = l_intDelegate, DelegateMode = 3 });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// Add time calc dictionary item, delegate execute every seconds.
    /// </summary>
    /// <param name="key">item key</param>
    /// <param name="duration">time over duration</param>
    /// <param name="l_floatDelegate">delegate</param>
    /// <returns>is add succeed</returns>
    public bool AddFrameDelegateToTimeCalc(string key, float duration, DelegateUtil.FloatDelegate l_floatDelegate = null)
    {
        if (TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot add key:" + key + " to time calc cause key already exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Add(new TimeCalc() { key = key, IsOverTime = true, StartTime = -1, OverTimeDuration = duration, m_TimeCalcFloatDelegate = l_floatDelegate, DelegateMode = 4 });

        StartCalc(key);
        return true;
    }

    /// <summary>
    /// Remove time calc dictionary item.
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is remove succeed</returns>
    public bool RemoveFromTimeCalc(string key)
    {
        StopCalc(key);

        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("Cannot remove key:" + key + " to time calc cause key not exist.");
            return false;
        }

        TimeCalc.m_timeCalcList.Remove(TimeCalc.GetTimeCalc(key));
        return true;
    }

    /// <summary>
    /// IsTimeCalcKeyExist
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is key exist</returns>
    public bool IsTimeCalcKeyExist(string key)
    {
        return TimeCalc.ContainsKey(key);
    }

    /// <summary>
    /// Start time calc
    /// </summary>
    /// <param name="key">item key</param>
    private void StartCalc(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return;
        }

        TimeCalc.GetTimeCalc(key).IsOverTime = false;
        TimeCalc.GetTimeCalc(key).StartTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Stop time calc
    /// </summary>
    /// <param name="key">item key</param>
    private void StopCalc(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return;
        }

        TimeCalc.GetTimeCalc(key).IsOverTime = true;
        TimeCalc.GetTimeCalc(key).StartTime = -1;
    }

    /// <summary>
    /// Is over time
    /// </summary>
    /// <param name="key">item key</param>
    /// <returns>is over</returns>
    public bool IsCalcTimeOver(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return false;
        }

        if (TimeCalc.GetTimeCalc(key).StartTime < 0)
        {
            Debug.LogError("Time calc key:" + key + " stopped or never start.");
            return false;
        }

        return TimeCalc.GetTimeCalc(key).IsOverTime;
    }

    public float GetCalcTime(string key)
    {
        if (!TimeCalc.ContainsKey(key))
        {
            Debug.LogError("key:" + key + " not exist.");
            return -1;
        }

        if (TimeCalc.GetTimeCalc(key).StartTime < 0)
        {
            Debug.LogError("Time calc key:" + key + " stopped or never start.");
            return -1;
        }

        return Time.realtimeSinceStartup - TimeCalc.GetTimeCalc(key).StartTime;
    }

    #endregion

    #region Animation

    public static int GetAnimatorPlayingHash(Animator p_animator)
    {
        return p_animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
    }

    #endregion
}
