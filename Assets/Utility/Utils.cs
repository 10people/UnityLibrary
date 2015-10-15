using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Utils : MonoBehaviour
{
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

}
