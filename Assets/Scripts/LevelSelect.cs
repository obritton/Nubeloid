using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {
	
	void OnGUI()
	{
		for( int i = 0; i < 18; i++ )
		{
			int xPos = i % 3;
			int yPos = i / 3;
			
			int level = i + 1;
			if( GUI.Button ( new Rect( 8 + xPos * 105, 10 + yPos * 78, 95, 70 ), "" + level ))
				loadLevel( level );
		}
	}
	
	void loadLevel( int level )
	{
		TextAsset configData = Resources.Load ("levelconfigs/config" + level) as TextAsset;
		PuzzleFactory.levelConfig = new LevelConfig(configData);
		Application.LoadLevel("4_AnyHeight");
	}
}
