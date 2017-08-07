using UnityEngine;
using System.Collections;

public class SimpleMainMenu : MonoBehaviour {
	
	public int speed = 15;
	public GUIStyle titleStyle;
	public GUIStyle buttonStyle;
	
	public GameObject cubeloid;
	public GameObject reflection;
	float offset = 1;
	
	void OnGUI(){
		if( GUI.Button( new Rect(0, 0 + offset, Screen.width, Screen.height-45), "PLAY", buttonStyle ))
		{
			if( !ExplosionTransition.isExploding )
			{
				//EndlessLevelsHandler.buildRandomLevel();
				
				ExplosionTransition explodeScript = (ExplosionTransition)GetComponent<ExplosionTransition>();
				StartCoroutine( explodeScript.doExplosionTransition());
			}
		}
		
		if( ExplosionTransition.isExploding )
			offset*=1.1f;
		
		GUI.Label( new Rect(0, Screen.height - 100 + offset, Screen.width, 100), "CUBELOID", titleStyle);
	}
	
	void Update()
	{
		cubeloid.transform.Rotate( 0, Time.deltaTime * speed, 0);
		reflection.transform.Rotate( 0, Time.deltaTime * speed, 0);
	}
}
