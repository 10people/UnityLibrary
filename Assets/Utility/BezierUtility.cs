using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierUtility
{
    /// <summary>
    /// a specific point in bezier curve.
    /// </summary>
    /// <param name="t_Para">bezier curve t_Para to get a specific point</param>
    /// <param name="m_Vector2List"></param>
    public static Vector2 GetBezierPoint(double t_Para, List<Vector2> m_Vector2List)
    {
        if (t_Para < 0 || t_Para > 1)
        {
            Debug.LogError("Not correct bezier t_Para:" + t_Para + " when calc bezier point.");
            return Vector2.zero;
        }

        if (m_Vector2List == null || m_Vector2List.Count == 0)
        {
            Debug.LogError("Cannot calc bezier point cause point list is empty.");
            return Vector2.zero;
        }

        int n = m_Vector2List.Count - 1;
        Vector2 returnVector2 = Vector2.zero;
        for (int i = 0; i <= n; i++)
        {
            returnVector2 += EquationItem(n, i, m_Vector2List[i], t_Para);
        }

        return returnVector2;
    }

    public static Vector2 GetDerivativeBezierPoint(double t_Para, List<Vector2> m_Vector2List)
    {
        if (t_Para < 0 || t_Para > 1)
        {
            Debug.LogError("Not correct bezier t_Para:" + t_Para + " when calc bezier point.");
            return Vector2.zero;
        }

        if (m_Vector2List == null || m_Vector2List.Count == 0)
        {
            Debug.LogError("Cannot calc bezier point cause point list is empty.");
            return Vector2.zero;
        }

        int n = m_Vector2List.Count - 1;
        Vector2 returnVector2 = Vector2.zero;
        for (int i = 0; i <= n; i++)
        {
            returnVector2 += DerivativeEquationItem(n, i, m_Vector2List[i], t_Para);
        }

        return returnVector2;
    }

    private static Vector2 EquationItem(int n, int i, Vector2 p, double t)
    {
        if (i > n || t < 0 || t > 1)
        {
            Debug.LogError("Got error in calc Bezier equation.");
            return Vector2.zero;
        }

        double kine = (Factorial(n) / (Factorial(i) * Factorial(n - i)) * Math.Pow(1 - t, n - i) * Math.Pow(t, i));
        return new Vector2((float)kine * p.x, (float)kine * p.y);
    }

    private static Vector2 DerivativeEquationItem(int n, int i, Vector2 p, double t)
    {
        if (i > n || t < 0 || t > 1)
        {
            Debug.LogError("Got error in calc Bezier equation.");
            return Vector2.zero;
        }

        double kine = (Factorial(n) / (Factorial(i) * Factorial(n - i)) * (i * Math.Pow(t, i - 1) - n * Math.Pow(t, n - 1)));
        return new Vector2((float)kine * p.x, (float)kine * p.y);
    }

    private static double Factorial(double x)
    {
        if (x < 0)
        {
            Debug.LogError("Factorial calc error.");
            return -1;
        }

        if (x == 0 || x == 1)
        {
            return 1;
        }
        else
        {
            return x * Factorial(x - 1);
        }
    }
}
