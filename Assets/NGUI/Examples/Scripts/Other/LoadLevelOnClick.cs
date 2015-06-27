using UnityEngine;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{
	public string levelName;

	void OnClick ()
	{
		if (!string.IsNullOrEmpty(levelName))
		{
			Debug.LogError( "NGUI.LoadLevelOnClick( " + levelName + " )" );

			Application.LoadLevel(levelName);
		}
	}
}