using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Utility : MonoBehaviour
{
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
    /// Set parent's child num to specific num.
    /// </summary>
    /// <param name="parentTransform">parent</param>
    /// <param name="prefabObject">child prefab</param>
    /// <param name="num">specific num</param>
    public static void AddOrDelItem(Transform parentTransform, GameObject prefabObject, int num)
    {
        if (num < 0)
        {
            Debug.LogError("Num should not be nagative, num:"+num);
            return;
        }

        if (parentTransform.childCount > num)
        {
            while (parentTransform.childCount!=num)
            {
                var child = parentTransform.GetChild(0);
                child.parent = null;
                Destroy(child);
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

                child.transform.parent = parentTransform;
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
        targetChild.transform.rotation = new Quaternion(0, 0, 0, 0);
        targetChild.transform.localScale = Vector3.one;
        targetChild.gameObject.SetActive(true);
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
}
