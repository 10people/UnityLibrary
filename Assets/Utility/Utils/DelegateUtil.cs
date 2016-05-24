using UnityEngine;
using System.Collections;

public class DelegateUtil
{

    #region Delegate without ret

    public delegate void VoidDelegate();

    public delegate void IntDelegate(int p_int);

    public delegate void FloatDelegate(float p_float);

    public delegate void StringDelegate(string p_str);

    public delegate void Vector2Delegate(Vector2 p_vec2);

    public delegate void Vector3Delegate(Vector3 p_vec3);

    #endregion

}
