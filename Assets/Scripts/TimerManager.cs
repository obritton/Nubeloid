using UnityEngine;
using System.Collections;

public class TimerManager : MonoBehaviour {
	
	public int totalTime = 90;
	public int currentTime = 0;
	
	public GUIStyle timeStyle;
	
	void Start()
	{
		StartCoroutine( "decrementCounter" );
	}
	
	public void stopCounter()
	{
		StopCoroutine("decrementCounter");
	}
	
	IEnumerator decrementCounter()
	{
		for( int i = 0; i < currentTime; i++ )
		{
			yield return new WaitForSeconds( 1 );
			currentTime--;
		}
	}
	
	void OnGUI()
	{
		GUI.Label( new Rect( 10, 0, 100, 40), "" + currentTime, timeStyle );
	}
}
