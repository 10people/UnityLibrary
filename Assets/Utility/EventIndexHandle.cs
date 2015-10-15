using UnityEngine;
using System.Collections;

public class EventIndexHandle : MonoBehaviour 
{
	public delegate void TouchedSend(int index);
	public event TouchedSend m_Handle;
	public int m_SendIndex;
	void Start () {
	
	}
 
	public void OnClick () 
	{
		if (m_Handle!= null)
		m_Handle (m_SendIndex);
	}   
}
